namespace EventFlow.Identity.Api.Contracts.Authorization;

public sealed class CheckPermissionRequest
{
    public Guid UserId { get; set; }

    public Guid EventId { get; set; }

    public string Permission { get; set; } = string.Empty;
}
