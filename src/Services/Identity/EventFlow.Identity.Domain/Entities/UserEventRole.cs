using EventFlow.Identity.Domain.Common;
namespace EventFlow.Identity.Domain.Entities
{
    public class UserEventRole : BaseEntity
    {
        private UserEventRole()
        {
        }

        public UserEventRole(Guid userId,Guid eventId,Guid roleId)
        {
            UserId = userId;
            EventId = eventId;
            RoleId = roleId;
            AssignedAt = DateTime.UtcNow;
        }

        public Guid UserId { get; private set; }
        public Guid EventId { get; private set; }
        public Guid RoleId { get; private set; }
        
        public DateTime AssignedAt { get; private set; }
        public User User { get; private set; } = null!;
        public Role Role { get; private set; } = null!;
    }
}
