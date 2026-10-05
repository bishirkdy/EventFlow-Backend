using EventFlow.SharedKernel.Exceptions;

namespace EventFlow.Registration.Application.Features.Participants.Queries.GetParticipantById;

public sealed class GetParticipantByIdQueryHandler(IParticipantRepository participants,EventFlow.Security.Authentication.ICurrentUserService user,IEventRegistrationAccessService access): IRequestHandler<GetParticipantByIdQuery, ParticipantDto>
{
    public async Task<ParticipantDto> Handle(GetParticipantByIdQuery query, CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(query.EventId,user.UserId, cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var participant = await participants.GetByIdAsync(
            query.EventId,
            query.ParticipantId,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (participant is null)
        {
            throw new NotFoundException("Participant not found.");
        }

        return participant.ToDto();
    }
}
