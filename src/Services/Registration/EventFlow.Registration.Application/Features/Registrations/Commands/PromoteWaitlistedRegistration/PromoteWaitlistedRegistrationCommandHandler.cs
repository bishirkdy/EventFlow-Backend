using AutoMapper;
using EventFlow.SharedKernel.Exceptions;
using EventFlow.Security.Authentication;


namespace EventFlow.Registration.Application.Features.Registrations.Commands.PromoteWaitlistedRegistration;

public sealed class PromoteWaitlistedRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IRegistrationFormRepository forms,
    IUnitOfWork unitOfWork,
    ICurrentUserService user,
    IEventRegistrationAccessService access,
    IMapper mapper)
    : IRequestHandler<PromoteWaitlistedRegistrationCommand, RegistrationDto>
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

        var form = await forms.GetByEventIdAsync(command.EventId, asNoTracking: true, cancellationToken: cancellationToken);

        if (form?.CapacityMode == CapacityMode.Limited && form.Capacity.HasValue &&
            await registrations.CountByStatusAsync(command.EventId,RegistrationStatus.Approved,cancellationToken) >= form.Capacity.Value)
        {
            throw new ConflictException("No available capacity.");
        }

        registration.Approve();

        if (registration.Participant is not null &&
            registration.Ticket is null)
        {
            var ticket = Ticket.Issue(
                registration.Id,
                registration.Participant.Id,
                registration.ScopeType,
                registration.ScopeId);

            registration.AttachTicket(ticket);
            registrations.AddTicket(ticket);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<RegistrationDto>(registration);
    }
}
