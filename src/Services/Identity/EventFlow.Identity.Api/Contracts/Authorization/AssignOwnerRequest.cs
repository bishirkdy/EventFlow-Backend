namespace EventFlow.Identity.Api.Contracts.Authorization;

public sealed class AssignOwnerRequest
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
}
