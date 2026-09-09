namespace EventFlow.Event.Api.Requests.Sections
{
    public sealed record UpdateSectionRequest(
        string Name,
        string? Description,
        int DisplayOrder);
}
