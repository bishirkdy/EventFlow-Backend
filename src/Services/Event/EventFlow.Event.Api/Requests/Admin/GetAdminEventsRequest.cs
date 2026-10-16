using EventFlow.Event.Domain.Enums;

namespace EventFlow.Event.Api.Requests.Admin;

public sealed class GetAdminEventsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public EventStatus? Status { get; init; }
    public string? Search { get; init; }
}
