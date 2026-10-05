namespace EventFlow.Identity.Application.Features.Queries.GetEventTeam;

public sealed record EventTeamMemberResponse(
    Guid UserId,
    string Email,
    string UserName,
    string FirstName,
    string LastName,
    IReadOnlyList<EventTeamRoleResponse> Roles);
