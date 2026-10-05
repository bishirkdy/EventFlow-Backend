using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.WaitlistRegistration;

public sealed class WaitlistRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        WaitlistRegistrationCommand,
        RegistrationDto>
{
    public async Task<RegistrationDto> Handle(
        WaitlistRegistrationCommand command,
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

        if (registration.Status is
            RegistrationStatus.Approved or
            RegistrationStatus.Cancelled)
        {
            throw new ConflictException("Registration cannot be waitlisted.");
        }

        if (registration.Status == RegistrationStatus.Waitlisted)
        {
            return registration.ToDto();
        }

        registration.Status = RegistrationStatus.Waitlisted;
        registration.WaitlistedAtUtc = DateTime.UtcNow;

        var currentWaitlistedCount =
            await registrations.CountByStatusAsync(
                command.EventId,
                RegistrationStatus.Waitlisted,
                cancellationToken);

        registration.WaitlistPosition =
            currentWaitlistedCount + 1;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return registration.ToDto();
    }
}
