namespace EventFlow.Event.Api.Requests.Sections
{
    public sealed record CreateSectionRequest(
        string Name,
        string? Description,
        int DisplayOrder);
}
