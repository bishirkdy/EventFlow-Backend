using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Contracts.RegistrationForms;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;

namespace EventFlow.Registration.Application.Features.RegistrationForms.Commands.UpsertRegistrationForm;

public sealed class UpsertRegistrationFormCommandHandler(
    IRegistrationFormRepository forms,
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        UpsertRegistrationFormCommand,
        RegistrationFormDto>
{
    public async Task<RegistrationFormDto> Handle(
        UpsertRegistrationFormCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var request = command.Request;

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
            throw new ConflictException(
                "One or more registration fields do not belong to this form.");
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

                throw new ConflictException(
                    $"These fields already have registration answers and cannot be deleted: {string.Join(", ", usedLabels)}.");
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

        return ToDto(form, approvedCount, waitlistCount);
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
