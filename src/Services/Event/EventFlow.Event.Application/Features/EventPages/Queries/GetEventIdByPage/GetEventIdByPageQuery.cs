using MediatR;

namespace EventFlow.Event.Application.Features.EventPages.Queries.GetEventIdByPage;

public sealed record GetEventIdByPageQuery(Guid PageId) : IRequest<Guid?>;
