using EventFlow.SharedKernel.Exceptions;
using EventFlow.Security.Authentication;

namespace EventFlow.Registration.Application.Features.Registrations.Queries.GetRegistrationById;

public sealed class GetRegistrationByIdQueryHandler(IRegistrationRepository registrations,ICurrentUserService user,IEventRegistrationAccessService access)
    : IRequestHandler<GetRegistrationByIdQuery, RegistrationDto>
{
    public async Task<RegistrationDto> Handle(
        GetRegistrationByIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.OrganizerView &&
            !await access.CanManageRegistrationAsync(
                query.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var registration = await registrations.GetByIdAsync(
            query.EventId,
            query.RegistrationId,
            userId: query.OrganizerView ? null : user.UserId,
            includeParticipant: true,
            includeTicket: true,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (registration is null)
        {
            throw new NotFoundException("Registration not found.");
        }

        return registration.ToDto();
    }
}
