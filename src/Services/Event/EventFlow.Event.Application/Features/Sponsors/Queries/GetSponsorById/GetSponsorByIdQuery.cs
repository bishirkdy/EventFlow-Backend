using MediatR;
namespace EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorById;
public sealed record GetSponsorByIdQuery(Guid Id, Guid EventId) : IRequest<GetSponsorByIdResponse?>;
