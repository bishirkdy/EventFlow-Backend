namespace EventFlow.Identity.Api.Contracts.Authorization;

public sealed class AssignEventRoleRequest
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}
