using EventFlow.Identity.Domain.Common;

namespace EventFlow.Identity.Domain.Entities
{
    public class Permission : BaseEntity
    {
        private Permission()
        {
        }

        public Permission(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        // Permission → Roles
        public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    }
}
