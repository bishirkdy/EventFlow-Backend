using EventFlow.Identity.Application.DTOs.UserEventRoles;
using MediatR;

namespace EventFlow.Identity.Application.Features.EventTeam.Queries.GetMyEventPermissions;

public sealed record GetMyEventPermissionsQuery(Guid UserId,Guid EventId) : IRequest<EventAccessResponse>;
