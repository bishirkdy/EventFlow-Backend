namespace EventFlow.Event.Api.Requests.Sessions
{
    public sealed record UpdateSessionRequest(
        string Title,
        string? Description,
        string SessionType,
        int? Capacity);
}
