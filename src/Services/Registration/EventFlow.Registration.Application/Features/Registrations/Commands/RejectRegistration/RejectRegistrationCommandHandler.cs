using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.RejectRegistration;

public sealed class RejectRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    IWaitlistPromotionService promotions,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<RejectRegistrationCommand, RegistrationDto>
{
    public async Task<RegistrationDto> Handle(
        RejectRegistrationCommand command,
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

        if (registration.Status == RegistrationStatus.Cancelled)
        {
            throw new ConflictException(
                "Cancelled registration cannot be rejected.");
        }

        var previousStatus = registration.Status;
        var now = DateTime.UtcNow;

        registration.Status = RegistrationStatus.Rejected;
        registration.RejectionReason = command.Request.Reason.Trim();
        registration.RejectedAtUtc = now;
        registration.WaitlistPosition = null;
        registration.WaitlistedAtUtc = null;

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
