using EventFlow.Contracts.Common;


namespace EventFlow.Registration.Application.Features.Participants.Queries.GetEventParticipants;

public sealed record GetEventParticipantsQuery(
    Guid EventId,
    ParticipantStatus? Status,
    string? Search,
    int Page = 1,
    int PageSize = 20)
    : IRequest<ApiResponse<PaginatedResponse<ParticipantDto>>>;
