using EventFlow.Identity.Domain.Common;

namespace EventFlow.Identity.Domain.Entities
{
    public class Role : BaseEntity
    {
        private Role()
        {
        }

        public Role(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; } = string.Empty;

        public string Description { get; private set; } = string.Empty;

        // Role → Permissions
        public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

        // Role → Users within events
        public ICollection<UserEventRole> UserEventRoles { get; private set; } = new List<UserEventRole>();
    }
}
