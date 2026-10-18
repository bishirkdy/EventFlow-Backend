namespace EventFlow.Identity.Application.DTOs.UserEventRoles;

public sealed record EventAccessResponse(IReadOnlyList<string> Roles, IReadOnlyList<string> Permissions);
