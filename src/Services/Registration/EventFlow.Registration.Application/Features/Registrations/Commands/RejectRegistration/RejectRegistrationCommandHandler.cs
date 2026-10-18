
using AutoMapper;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.RejectRegistration;

public sealed class RejectRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    IWaitlistPromotionService promotions,
    ICurrentUserService user,
    IEventRegistrationAccessService access,
    IMapper mapper)
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

        registration.Reject(command.Request.Reason.Trim());

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (previousStatus == RegistrationStatus.Approved)
        {
            await promotions.PromoteWaitlistedUntilCapacityAsync(
                command.EventId,
                cancellationToken);
        }

        return mapper.Map<RegistrationDto>(registration);
    }
}
