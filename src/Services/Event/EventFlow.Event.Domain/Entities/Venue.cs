using EventFlow.Event.Domain.Common;


namespace EventFlow.Event.Domain.Entities
{
    public sealed class Venue : Entity
    {
        public Guid EventId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string? Address { get; private set; }
        public int Capacity { get; private set; }
        public bool IsActive { get; private set; }

        private Venue()
        {
            // Required by EF Core
        }

        public Venue(Guid eventId,string name,string? description,string? address,int capacity)
        {
            EventId = eventId;
            Name = name;
            Description = description;
            Address = address;
            Capacity = capacity;
            IsActive = true;
        }

        public void Update(string name,string? description,string? address,int capacity)
        {
            Name = name;
            Description = description;
            Address = address;
            Capacity = capacity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCapacity(int capacity)
        {
            Capacity = capacity;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
