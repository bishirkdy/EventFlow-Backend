namespace EventFlow.Identity.Api.Contracts.Authorization
{
    public sealed record CheckPermissionRequest(
        Guid UserId,
        Guid EventId,
        string Permission);
}
