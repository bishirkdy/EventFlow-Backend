using EventFlow.Contracts.Common;

namespace EventFlow.Registration.Application.Features.RegistrationForms.Commands.UpsertRegistrationForm;

public sealed class UpsertRegistrationFormCommandHandler(
    IRegistrationFormRepository forms,
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    ICurrentUserService user,
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
                Name = command.Request.Name.Trim(),
                Description = command.Request.Description?.Trim(),
                IsActive = command.Request.IsActive,
                CapacityMode = command.Request.CapacityMode,
                Capacity = command.Request.Capacity,
                EnableWaitlist = command.Request.EnableWaitlist,
                OpensAtUtc = command.Request.OpensAtUtc,
                ClosesAtUtc = command.Request.ClosesAtUtc
            };

            forms.Add(form);
        }
        else
        {
            form.Name = command.Request.Name.Trim();
            form.Description =
                command.Request.Description?.Trim();

            form.IsActive =
                command.Request.IsActive;

            form.CapacityMode =
                command.Request.CapacityMode;

            form.Capacity =
                command.Request.Capacity;

            form.EnableWaitlist =
                command.Request.EnableWaitlist;

            form.OpensAtUtc =
                command.Request.OpensAtUtc;

            form.ClosesAtUtc =
                command.Request.ClosesAtUtc;

            form.UpdatedAtUtc = now;
        }

        var fields = command.Request.Fields
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new RegistrationFormField
            {
                Id = x.Id.GetValueOrDefault(Guid.NewGuid()),
                RegistrationFormId = form.Id,
                FieldKey = x.FieldKey.Trim(),
                Label = x.Label.Trim(),
                FieldType = x.FieldType,
                IsRequired = x.IsRequired,
                DisplayOrder = x.DisplayOrder,
                OptionsJson = x.OptionsJson,
                ValidationJson = x.ValidationJson
            })
            .ToList();

        forms.ReplaceFields(form, fields);

        await unitOfWork.SaveChangesAsync(
            CancellationToken.None);

        var approvedCount =
            await registrations.CountByStatusAsync(
                command.EventId,
                RegistrationStatus.Approved,
                cancellationToken);
                
        var waitlistCount =
            await registrations.CountByStatusAsync(
                command.EventId,
                RegistrationStatus.Waitlisted,
                cancellationToken);

        return ApiResponse<RegistrationFormDto>.Success(
            new RegistrationFormDto
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
            },
            "Registration form saved.");
    }
}