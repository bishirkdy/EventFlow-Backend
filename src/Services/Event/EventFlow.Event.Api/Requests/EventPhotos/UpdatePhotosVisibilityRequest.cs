namespace EventFlow.Event.Api.Requests.EventPhotos;

public sealed record UpdatePhotosVisibilityRequest(IReadOnlyList<Guid> PhotoIds, bool IsVisible);
