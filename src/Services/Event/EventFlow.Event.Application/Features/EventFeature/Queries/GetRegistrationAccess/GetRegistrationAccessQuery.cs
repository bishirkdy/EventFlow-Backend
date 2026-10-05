using MediatR;

namespace EventFlow.Event.Application.Features.EventFeature.Queries.GetRegistrationAccess;

public sealed record GetRegistrationAccessQuery(Guid EventId, Guid UserId)
    : IRequest<bool>;
