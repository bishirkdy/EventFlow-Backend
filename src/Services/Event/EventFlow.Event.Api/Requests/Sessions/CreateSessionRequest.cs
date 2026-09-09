namespace EventFlow.Event.Api.Requests.Sessions
{
    public sealed record CreateSessionRequest(
        Guid SectionId,
        string Title,
        string? Description,
        string SessionType,
        int? Capacity);
}
