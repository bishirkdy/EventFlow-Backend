using EventFlow.Contracts.Common;
using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.CreateRegistration;

public sealed class CreateRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IRegistrationFormRepository forms,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        CreateRegistrationCommand,
        ApiResponse<RegistrationDto>>
{
    public async Task<ApiResponse<RegistrationDto>> Handle(
        CreateRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        if (!user.IsAuthenticated)
        {
            return ApiResponse<RegistrationDto>.Fail(
                ["Authentication is required."]);
        }

        if (!await access.IsRegistrationFeatureEnabledAsync(
                command.EventId,
                cancellationToken))
        {
            return ApiResponse<RegistrationDto>.Fail(
                ["Registration is not enabled for this event."]);
        }

        var form = await forms.GetByEventIdAsync(command.EventId,includeFields: true,asNoTracking: true, cancellationToken: cancellationToken);

        var now = DateTime.UtcNow;

        if (form is not null)
        {
            if (!form.IsActive)
            {
                return ApiResponse<RegistrationDto>.Fail(
                    ["Registration is closed."]);
            }

            if (form.OpensAtUtc.HasValue &&
                now < form.OpensAtUtc.Value)
            {
                return ApiResponse<RegistrationDto>.Fail(
                    ["Registration has not opened yet."]);
            }

            if (form.ClosesAtUtc.HasValue &&
                now > form.ClosesAtUtc.Value)
            {
                return ApiResponse<RegistrationDto>.Fail(
                    ["Registration is closed."]);
            }

            var answerErrors = RegistrationFormAnswerValidator.Validate(
                form,
                command.Request.Answers);

            if (answerErrors.Count > 0)
            {
                return ApiResponse<RegistrationDto>.Fail(answerErrors);
            }
        }

        if (await registrations.HasActiveRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<RegistrationDto>.Fail(
                ["You already have an active registration for this event."]);
        }

        var approvedCount = await registrations.CountByStatusAsync(
            command.EventId,
            RegistrationStatus.Approved,
            cancellationToken);

        var status = RegistrationStatus.Pending;
        int? waitlistPosition = null;

        if (form?.CapacityMode == CapacityMode.Limited &&
            form.Capacity.HasValue &&
            approvedCount >= form.Capacity.Value)
        {
            if (!form.EnableWaitlist)
            {
                return ApiResponse<RegistrationDto>.Fail(
                    ["Event capacity has been reached."]);
            }

            status = RegistrationStatus.Waitlisted;

            var waitlistedCount = await registrations.CountByStatusAsync(
                command.EventId,
                RegistrationStatus.Waitlisted,
                cancellationToken);

            waitlistPosition = waitlistedCount + 1;
        }

        var registrationId = Guid.NewGuid();

        var registration = new RegistrationEntity
        {
            Id = registrationId,
            EventId = command.EventId,
            UserId = user.UserId,
            RegistrationNumber =
                $"REG-{now:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}",
            Status = status,
            RegisteredAtUtc = now,
            WaitlistedAtUtc =
                status == RegistrationStatus.Waitlisted ? now : null,
            WaitlistPosition = waitlistPosition,
            CreatedAtUtc = now
        };

        registration.Participant = new Participant
        {
            Id = Guid.NewGuid(),
            RegistrationId = registrationId,
            EventId = command.EventId,
            UserId = user.UserId,
            ParticipantNumber =
                $"P-{now:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}",
            FirstName = command.Request.FirstName.Trim(),
            LastName = command.Request.LastName.Trim(),
            Email = command.Request.Email.Trim(),
            Phone = command.Request.Phone?.Trim(),
            Organization = command.Request.Organization?.Trim(),
            Designation = command.Request.Designation?.Trim(),
            CreatedAtUtc = now,
            Registration = registration
        };

        foreach (var answer in command.Request.Answers)
        {
            registration.Answers.Add(
                new RegistrationAnswer
                {
                    Id = Guid.NewGuid(),
                    RegistrationId = registrationId,
                    RegistrationFormFieldId = answer.Key,
                    Value = answer.Value ?? string.Empty
                });
        }

        registrations.Add(registration);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<RegistrationDto>.Success(
            registration.ToDto(),
            status == RegistrationStatus.Waitlisted
                ? "Registration submitted and added to the waitlist."
                : "Registration submitted successfully.");
    }
}
