using EventFlow.Identity.Domain.Enums;
using EventFlow.SharedKernel.Domain;

namespace EventFlow.Identity.Domain.Entities
{
    public class PhotographerInvitation : Entity
    {
        private PhotographerInvitation()
        {
        }

        public PhotographerInvitation(Guid eventId, string email, Guid roleId, Guid createdBy)
        {
            EventId = eventId;
            Email = email.ToLowerInvariant();
            RoleId = roleId;
            CreatedBy = createdBy;
            Token = Guid.NewGuid().ToString("N");
            ExpiresAt = DateTime.UtcNow.AddDays(7);
            Status = InvitationStatus.Pending;
        }

        public Guid EventId { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public Guid RoleId { get; private set; }
        public Guid CreatedBy { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public DateTime? AcceptedAt { get; private set; }
        public InvitationStatus Status { get; private set; }
        public Guid? AcceptedByUserId { get; private set; }

        public Role Role { get; private set; } = null!;
        public User CreatedByUser { get; private set; } = null!;
        public User? AcceptedByUser { get; private set; }

        public void Accept(Guid userId)
        {
            if (Status != InvitationStatus.Pending)
                throw new InvalidOperationException("Invitation has already been processed.");

            if (DateTime.UtcNow > ExpiresAt)
                throw new InvalidOperationException("Invitation has expired.");

            Status = InvitationStatus.Accepted;
            AcceptedAt = DateTime.UtcNow;
            AcceptedByUserId = userId;
            SetUpdatedAt();
        }

        public void Revoke()
        {
            if (Status == InvitationStatus.Accepted)
                throw new InvalidOperationException("Cannot revoke an accepted invitation.");

            Status = InvitationStatus.Revoked;
            SetUpdatedAt();
        }

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }

}