namespace EventFlow.Messaging.Events
{
    public sealed record AttendanceStaffInvitationAcceptedEvent : IntegrationEvent
    {
        public Guid InvitationId { get; init; }
        public Guid UserId { get; init; }
        public int? ScopeType { get; init; }
        public Guid? ScopeId { get; init; }
    }
}
