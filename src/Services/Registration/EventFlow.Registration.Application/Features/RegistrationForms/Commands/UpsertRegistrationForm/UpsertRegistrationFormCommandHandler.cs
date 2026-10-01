using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Contracts.RegistrationForms;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Application.Features.RegistrationForms.Commands.UpsertRegistrationForm;

public sealed class UpsertRegistrationFormCommandHandler(
    IRegistrationFormRepository forms,
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        UpsertRegistrationFormCommand,
        ApiResponse<RegistrationFormDto>>
{
    public async Task<ApiResponse<RegistrationFormDto>> Handle(
        UpsertRegistrationFormCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<RegistrationFormDto>.Fail(
                ["You do not have permission."]);
        }

        var request = command.Request;
        var validationError = ValidateRequest(request);

        if (validationError is not null)
        {
            return ApiResponse<RegistrationFormDto>.Fail([validationError]);
        }

        var now = DateTime.UtcNow;
        var form = await forms.GetByEventIdAsync(
            command.EventId,
            includeFields: true,
            cancellationToken: cancellationToken);

        if (form is null)
        {
            form = new RegistrationForm
            {
                Id = Guid.NewGuid(),
                EventId = command.EventId,
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                IsActive = request.IsActive,
                CapacityMode = request.CapacityMode,
                Capacity = request.Capacity,
                EnableWaitlist = request.EnableWaitlist,
                OpensAtUtc = request.OpensAtUtc,
                ClosesAtUtc = request.ClosesAtUtc
            };

            forms.Add(form);
        }
        else
        {
            form.Name = request.Name.Trim();
            form.Description = request.Description?.Trim();
            form.IsActive = request.IsActive;
            form.CapacityMode = request.CapacityMode;
            form.Capacity = request.Capacity;
            form.EnableWaitlist = request.EnableWaitlist;
            form.OpensAtUtc = request.OpensAtUtc;
            form.ClosesAtUtc = request.ClosesAtUtc;
            form.UpdatedAtUtc = now;
        }

        var existingFields = form.Fields.ToDictionary(x => x.Id);
        var incomingIds = request.Fields
            .Where(x => x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .ToHashSet();

        var invalidIds = incomingIds
            .Where(id => !existingFields.ContainsKey(id))
            .ToArray();

        if (invalidIds.Length > 0)
        {
            return ApiResponse<RegistrationFormDto>.Fail(
                ["One or more registration fields do not belong to this form."]);
        }

        var removedIds = existingFields.Keys
            .Where(id => !incomingIds.Contains(id))
            .ToArray();

        if (removedIds.Length > 0)
        {
            var usedFieldIds = await forms.GetFieldIdsWithAnswersAsync(
                removedIds,
                cancellationToken);

            if (usedFieldIds.Count > 0)
            {
                var usedLabels = existingFields
                    .Where(x => usedFieldIds.Contains(x.Key))
                    .Select(x => x.Value.Label)
                    .ToArray();

                return ApiResponse<RegistrationFormDto>.Fail(
                    [$"These fields already have registration answers and cannot be deleted: {string.Join(", ", usedLabels)}."]);
            }
        }

        var fields = request.Fields
            .OrderBy(x => x.DisplayOrder)
            .Select((x, index) => new RegistrationFormField
            {
                Id = x.Id ?? Guid.Empty,
                RegistrationFormId = form.Id,
                FieldKey = x.FieldKey.Trim(),
                Label = x.Label.Trim(),
                FieldType = x.FieldType,
                IsRequired = x.IsRequired,
                DisplayOrder = index,
                OptionsJson = NormalizeJson(x.OptionsJson),
                ValidationJson = NormalizeJson(x.ValidationJson)
            })
            .ToList();

        forms.ReplaceFields(form, fields);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var approvedCount = await registrations.CountByStatusAsync(
            command.EventId,
            RegistrationStatus.Approved,
            cancellationToken);

        var waitlistCount = await registrations.CountByStatusAsync(
            command.EventId,
            RegistrationStatus.Waitlisted,
            cancellationToken);

        return ApiResponse<RegistrationFormDto>.Success(
            ToDto(form, approvedCount, waitlistCount),
            "Registration form saved.");
    }

    private static string? ValidateRequest(UpsertRegistrationFormRequest request)
    {
        if (request.CapacityMode == CapacityMode.Limited &&
            (!request.Capacity.HasValue || request.Capacity.Value < 1))
        {
            return "A positive capacity is required when capacity mode is limited.";
        }

        if (request.CapacityMode == CapacityMode.Unlimited && request.EnableWaitlist)
        {
            return "Waitlist can only be enabled when capacity is limited.";
        }

        if (request.OpensAtUtc.HasValue &&
            request.ClosesAtUtc.HasValue &&
            request.OpensAtUtc.Value >= request.ClosesAtUtc.Value)
        {
            return "Registration opening time must be before the closing time.";
        }

        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in request.Fields)
        {
            if (!keys.Add(field.FieldKey.Trim()))
            {
                return $"The field key '{field.FieldKey.Trim()}' is duplicated.";
            }

            if (!Enum.IsDefined(field.FieldType))
            {
                return $"Field '{field.Label}' has an invalid field type.";
            }

            var jsonError = ValidateFieldJson(field);
            if (jsonError is not null)
            {
                return jsonError;
            }
        }

        return null;
    }

    private static string? ValidateFieldJson(RegistrationFormFieldRequest field)
    {
        var isChoice = field.FieldType is
            RegistrationFieldType.Select or
            RegistrationFieldType.Radio;

        if (isChoice)
        {
            if (string.IsNullOrWhiteSpace(field.OptionsJson))
            {
                return $"Field '{field.Label}' requires options.";
            }

            try
            {
                using var document = System.Text.Json.JsonDocument.Parse(field.OptionsJson);

                if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Array)
                {
                    return $"Options for '{field.Label}' must be a JSON array.";
                }

                var options = document.RootElement.EnumerateArray().ToArray();

                if (options.Length == 0 || options.Any(x => x.ValueKind != System.Text.Json.JsonValueKind.String || string.IsNullOrWhiteSpace(x.GetString())))
                {
                    return $"Options for '{field.Label}' must contain at least one non-empty string.";
                }
            }
            catch (System.Text.Json.JsonException)
            {
                return $"Options for '{field.Label}' contain invalid JSON.";
            }
        }
        else if (!string.IsNullOrWhiteSpace(field.OptionsJson))
        {
            return $"Field '{field.Label}' does not support options.";
        }

        if (string.IsNullOrWhiteSpace(field.ValidationJson))
        {
            return null;
        }

        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(field.ValidationJson);

            if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object)
            {
                return $"Validation rules for '{field.Label}' must be a JSON object.";
            }
        }
        catch (System.Text.Json.JsonException)
        {
            return $"Validation rules for '{field.Label}' contain invalid JSON.";
        }

        return null;
    }

    private static string? NormalizeJson(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static RegistrationFormDto ToDto(
        RegistrationForm form,
        int approvedCount,
        int waitlistCount)
    {
        return new RegistrationFormDto
        {
            Id = form.Id,
            EventId = form.EventId,
            Name = form.Name,
            Description = form.Description,
            IsActive = form.IsActive,
            CapacityMode = form.CapacityMode,
            Capacity = form.Capacity,
            ApprovedCount = approvedCount,
            WaitlistCount = waitlistCount,
            EnableWaitlist = form.EnableWaitlist,
            OpensAtUtc = form.OpensAtUtc,
            ClosesAtUtc = form.ClosesAtUtc,
            Fields = form.Fields
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new RegistrationFormFieldDto
                {
                    Id = x.Id,
                    FieldKey = x.FieldKey,
                    Label = x.Label,
                    FieldType = x.FieldType,
                    IsRequired = x.IsRequired,
                    DisplayOrder = x.DisplayOrder,
                    OptionsJson = x.OptionsJson,
                    ValidationJson = x.ValidationJson
                })
                .ToList()
        };
    }
}
