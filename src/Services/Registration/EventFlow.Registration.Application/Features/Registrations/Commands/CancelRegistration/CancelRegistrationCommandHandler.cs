using AutoMapper;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.CancelRegistration;

public sealed class CancelRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    IWaitlistPromotionService promotions,
    ICurrentUserService user,
    IMapper mapper)
    : IRequestHandler<CancelRegistrationCommand, RegistrationDto>
{
    public async Task<RegistrationDto> Handle(CancelRegistrationCommand command, CancellationToken cancellationToken)
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
            return mapper.Map<RegistrationDto>(registration);
        }

        var previousStatus = registration.Status;

        registration.Cancel(command.Request.Reason?.Trim());

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
