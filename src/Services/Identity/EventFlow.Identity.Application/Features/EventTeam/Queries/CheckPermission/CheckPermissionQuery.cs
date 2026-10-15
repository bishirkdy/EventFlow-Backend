using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Queries.CheckPermission;

public sealed record CheckPermissionQuery(
    Guid UserId,
    Guid EventId,
    string Permission)
    : IRequest<bool>;
