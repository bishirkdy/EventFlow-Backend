namespace EventFlow.Event.Application.Features.Events.Common
{
    public sealed record EventImageResponse(
        Guid Id,
        string Url,
        string OriginalFileName,
        string ContentType,
        long SizeBytes,
        int DisplayOrder);
}
