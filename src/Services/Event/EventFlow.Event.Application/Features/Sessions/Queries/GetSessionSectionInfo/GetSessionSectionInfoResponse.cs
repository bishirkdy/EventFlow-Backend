namespace EventFlow.Event.Application.Features.Sessions.Queries.GetSessionSectionInfo
{
    public sealed record GetSessionSectionInfoResponse(
        bool Exists,
        Guid SectionId);
}
