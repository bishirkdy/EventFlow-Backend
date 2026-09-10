

using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities
{
    public sealed class NavigationMenu : Entity
    {
        public Guid EventId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;

        private NavigationMenu()
        {
            // Required by EF Core
        }

        public NavigationMenu(Guid eventId,string name,string location)
        {
            EventId = eventId;
            Name = name;
            Location = location;
        }

        public void Update(string name, string location)
        {
            Name = name;
            Location = location;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
