using AutoMapper;
using EventFlow.Security.Authentication;
using EventFlow.SharedKernel.Exceptions;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.ApproveRegistration;

public sealed class ApproveRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IRegistrationFormRepository forms,
    IUnitOfWork unitOfWork,
    ICurrentUserService user,
    IEventRegistrationAccessService access,
    IMapper mapper)
    : IRequestHandler<ApproveRegistrationCommand, RegistrationDto>
{
    public async Task<RegistrationDto> Handle(ApproveRegistrationCommand command, CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(command.EventId, user.UserId, cancellationToken))
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

        if (registration.Status is RegistrationStatus.Cancelled or RegistrationStatus.Rejected)
        {
            throw new ConflictException("Registration cannot be approved.");
        }

        if (registration.Status == RegistrationStatus.Approved)
        {
            return mapper.Map<RegistrationDto>(registration);
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
            throw new ConflictException("Event capacity has been reached.");
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
