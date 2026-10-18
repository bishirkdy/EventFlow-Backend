namespace EventFlow.Messaging.Events;

public sealed record EventInvitationCreatedEvent : IntegrationEvent
{
    public Guid InvitationId { get; init; }
    public string Email { get; init; } = string.Empty;
    public int InvitationType { get; init; }
    public int? ScopeType { get; init; }
    public Guid? ScopeId { get; init; }
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
