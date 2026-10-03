using EventFlow.Identity.Application.Abstractions.Repositories;
using MediatR;

namespace EventFlow.Identity.Application.Features.Queries.GetEventPhotographerInvitations
{
    public class GetEventPhotographerInvitationsQueryHandler : IRequestHandler<GetEventPhotographerInvitationsQuery, IReadOnlyList<GetEventPhotographerInvitationsResponse>>
    {
        private readonly IPhotographerInvitationRepository _invitationRepository;

        public GetEventPhotographerInvitationsQueryHandler(IPhotographerInvitationRepository invitationRepository)
        {
            _invitationRepository = invitationRepository;
        }

        public async Task<IReadOnlyList<GetEventPhotographerInvitationsResponse>> Handle(GetEventPhotographerInvitationsQuery request, CancellationToken cancellationToken)
        {
            var invitations = await _invitationRepository.GetByEventIdAsync(request.EventId, cancellationToken);

            return invitations.Select(i => new GetEventPhotographerInvitationsResponse(
                i.Id,
                i.Email,
                i.Role?.Name ?? string.Empty,
                i.Status,
                i.CreatedAt,
                i.ExpiresAt,
                i.AcceptedAt,
                i.AcceptedByUserId)).ToList();
        }
    }
}