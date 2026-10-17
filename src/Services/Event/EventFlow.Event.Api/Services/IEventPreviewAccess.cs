namespace EventFlow.Event.Api.Services;

// Decides who may read an event's website content before it is published.
public interface IEventPreviewAccess
{
    // True when the caller is a platform admin or an event member allowed to
    // manage the event (owner, organizer or event admin).
    Task<bool> CanPreviewAsync(Guid eventId, CancellationToken cancellationToken = default);

    // True when the event is published, or the caller can preview it.
    Task<bool> CanReadAsync(Guid eventId, CancellationToken cancellationToken = default);
}
