using EventFlow.Contracts.Common;


namespace EventFlow.Registration.Application.Features.Participants.Queries.GetEventParticipants;

public sealed class GetEventParticipantsQueryHandler(
    IParticipantRepository participants,
    ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        GetEventParticipantsQuery,
        ApiResponse<PaginatedResponse<ParticipantDto>>>
{
    public async Task<ApiResponse<PaginatedResponse<ParticipantDto>>> Handle(GetEventParticipantsQuery query, CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(query.EventId, user.UserId, cancellationToken))
        {
            return ApiResponse<PaginatedResponse<ParticipantDto>>.Fail(["You do not have permission."]);
        }

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var (items, totalCount) =
            await participants.GetPagedForEventAsync(
                query.EventId,
                query.Status,
                query.Search,
                page,
                pageSize,
                cancellationToken);

        return ApiResponse<PaginatedResponse<ParticipantDto>>.Success(
            new PaginatedResponse<ParticipantDto>
            {
                Items = items.Select(x => x.ToDto()).ToList(),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            });
    }
}
