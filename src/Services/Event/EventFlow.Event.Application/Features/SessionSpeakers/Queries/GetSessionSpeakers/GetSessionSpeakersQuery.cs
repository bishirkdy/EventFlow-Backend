using MediatR;
namespace EventFlow.Event.Application.Features.SessionSpeakers.Queries.GetSessionSpeakers;
public sealed record GetSessionSpeakersQuery(Guid EventId, Guid SessionId) : IRequest<IReadOnlyList<GetSessionSpeakersResponse>>;
