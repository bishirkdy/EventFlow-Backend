
using AutoMapper;
using EventFlow.SharedKernel.Exceptions;
using EventFlow.Security.Authentication;

namespace EventFlow.Registration.Application.Features.RegistrationForms.Commands.UpsertRegistrationForm;

public sealed class UpsertRegistrationFormCommandHandler(
    IRegistrationFormRepository forms,
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    ICurrentUserService user,
    IEventRegistrationAccessService access,
    IMapper mapper)
    : IRequestHandler<UpsertRegistrationFormCommand,RegistrationFormDto>
{
    public async Task<RegistrationFormDto> Handle(
        UpsertRegistrationFormCommand command, CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(command.EventId,user.UserId,cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var request = command.Request;

        var form = await forms.GetByEventIdAsync(
            command.EventId,
            includeFields: true,
            cancellationToken: cancellationToken);

        if (form is null)
        {
            form = new RegistrationForm(command.EventId);
            forms.Add(form);
        }

        form.Update(
            request.Name.Trim(),
            request.Description?.Trim(),
            request.IsActive,
            request.CapacityMode,
            request.Capacity,
            request.EnableWaitlist,
            request.OpensAtUtc,
            request.ClosesAtUtc);

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
            throw new ConflictException("One or more registration fields do not belong to this form.");
        }

        var removedIds = existingFields.Keys
            .Where(id => !incomingIds.Contains(id))
            .ToArray();

        if (removedIds.Length > 0)
        {
            var usedFieldIds = await forms.GetFieldIdsWithAnswersAsync(removedIds,cancellationToken);

            if (usedFieldIds.Count > 0)
            {
                var usedLabels = existingFields
                    .Where(x => usedFieldIds.Contains(x.Key))
                    .Select(x => x.Value.Label)
                    .ToArray();

                throw new ConflictException($"These fields already have registration answers and cannot be deleted: {string.Join(", ", usedLabels)}.");
            }
        }

        var fields = request.Fields
            .OrderBy(x => x.DisplayOrder)
            .Select((x, index) => x.Id.HasValue
                ? new RegistrationFormField(
                    x.Id.Value,
                    form.Id,
                    x.FieldKey.Trim(),
                    x.Label.Trim(),
                    x.FieldType,
                    x.IsRequired,
                    index,
                    NormalizeJson(x.OptionsJson),
                    NormalizeJson(x.ValidationJson))
                : new RegistrationFormField(
                    form.Id,
                    x.FieldKey.Trim(),
                    x.Label.Trim(),
                    x.FieldType,
                    x.IsRequired,
                    index,
                    NormalizeJson(x.OptionsJson),
                    NormalizeJson(x.ValidationJson)))
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

        var dto = mapper.Map<RegistrationFormDto>(form);
        dto.ApprovedCount = approvedCount;
        dto.WaitlistCount = waitlistCount;

        return dto;
    }

    private static string? NormalizeJson(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
