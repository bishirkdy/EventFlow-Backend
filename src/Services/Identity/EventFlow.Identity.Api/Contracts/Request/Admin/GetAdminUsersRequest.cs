namespace EventFlow.Identity.Api.Contracts.Request.Admin;

public sealed class GetAdminUsersRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
}
