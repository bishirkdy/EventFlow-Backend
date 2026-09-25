using MediatR;
namespace EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakersByEvent;
public sealed record GetSpeakersByEventQuery(Guid EventId) : IRequest<IReadOnlyList<GetSpeakersByEventResponse>>;
