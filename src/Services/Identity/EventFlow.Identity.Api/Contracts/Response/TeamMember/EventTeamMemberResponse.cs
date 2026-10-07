
namespace EventFlow.Identity.Api.Contracts.Response.TeamMember
{
    public sealed record EventTeamMemberResponse(
        Guid UserId,
        string Email,
        string UserName,
        string FirstName,
        string LastName,
        IReadOnlyList<EventTeamRoleResponse> Roles);
}
