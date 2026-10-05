using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.CancelRegistration;

public sealed class CancelRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    IWaitlistPromotionService promotions,
    EventFlow.Security.Authentication.ICurrentUserService user)
    : IRequestHandler<CancelRegistrationCommand, RegistrationDto>
{
    public async Task<RegistrationDto> Handle(
        CancelRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        var registration = await registrations.GetByIdAsync(
            command.EventId,
            command.RegistrationId,
            userId: user.UserId,
            includeParticipant: true,
            includeTicket: true,
            cancellationToken: cancellationToken);

        if (registration is null)
        {
            throw new NotFoundException("Registration not found.");
        }

        if (registration.Status == RegistrationStatus.Cancelled)
        {
            return registration.ToDto();
        }

        var previousStatus = registration.Status;
        var now = DateTime.UtcNow;

        registration.Status = RegistrationStatus.Cancelled;
        registration.CancellationReason =
            command.Request.Reason?.Trim();
        registration.CancelledAtUtc = now;

        if (registration.Participant is not null)
        {
            registration.Participant.Status = ParticipantStatus.Cancelled;
        }

        if (registration.Ticket is not null)
        {
            registration.Ticket.IsActive = false;
            registration.Ticket.RevokedAtUtc = now;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (previousStatus == RegistrationStatus.Approved)
        {
            await promotions.PromoteWaitlistedUntilCapacityAsync(
                command.EventId,
                cancellationToken);
        }

        return registration.ToDto();
    }
}
