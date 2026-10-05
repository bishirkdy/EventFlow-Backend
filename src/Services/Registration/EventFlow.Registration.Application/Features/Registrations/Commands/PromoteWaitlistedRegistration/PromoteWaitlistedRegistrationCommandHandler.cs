using System.Security.Cryptography;
using EventFlow.SharedKernel.Exceptions;


namespace EventFlow.Registration.Application.Features.Registrations.Commands.PromoteWaitlistedRegistration;

public sealed class PromoteWaitlistedRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IRegistrationFormRepository forms,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        PromoteWaitlistedRegistrationCommand,
        RegistrationDto>
{
    public async Task<RegistrationDto> Handle(
        PromoteWaitlistedRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var registration = await registrations.GetByIdAsync(
            command.EventId,
            command.RegistrationId,
            includeParticipant: true,
            includeTicket: true,
            cancellationToken: cancellationToken);

        if (registration is null)
        {
            throw new NotFoundException("Registration not found.");
        }

        if (registration.Status != RegistrationStatus.Waitlisted)
        {
            throw new ConflictException("Registration is not waitlisted.");
        }

        var form = await forms.GetByEventIdAsync(
            command.EventId,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (form?.CapacityMode == CapacityMode.Limited &&
            form.Capacity.HasValue &&
            await registrations.CountByStatusAsync(
                command.EventId,
                RegistrationStatus.Approved,
                cancellationToken) >= form.Capacity.Value)
        {
            throw new ConflictException("No available capacity.");
        }

        var now = DateTime.UtcNow;

        registration.Status = RegistrationStatus.Approved;
        registration.ApprovedAtUtc = now;
        registration.WaitlistedAtUtc = null;
        registration.WaitlistPosition = null;

        if (registration.Participant is not null &&
            registration.Ticket is null)
        {
            registration.Ticket = new Ticket
            {
                RegistrationId = registration.Id,
                ParticipantId = registration.Participant.Id,
                TicketNumber =
                    $"TKT-{now:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}",
                QrCodeValue =
                    Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                        .Replace("+", "-")
                        .Replace("/", "_")
                        .TrimEnd('='),
                IssuedAtUtc = now
            };
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return registration.ToDto();
    }
}
