using MediatR;
namespace EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakerById;
public sealed record GetSpeakerByIdQuery(Guid Id, Guid EventId) : IRequest<GetSpeakerByIdResponse?>;
