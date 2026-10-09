

namespace EventFlow.Messaging.Events
{
    public sealed record PhotographerInvitationCreatedEvent : IntegrationEvent
    {
        public Guid InvitationId { get; init; }
        public Guid EventId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string Token { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
    }
}
