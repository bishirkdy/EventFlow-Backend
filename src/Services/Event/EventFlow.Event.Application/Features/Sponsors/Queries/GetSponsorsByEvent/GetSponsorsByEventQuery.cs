using MediatR;
namespace EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorsByEvent;
public sealed record GetSponsorsByEventQuery(Guid EventId) : IRequest<IReadOnlyList<GetSponsorsByEventResponse>>;
