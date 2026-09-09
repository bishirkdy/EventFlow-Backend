using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities
{
    public sealed class Section : Entity
    {
        public Guid EventId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public int DisplayOrder { get; private set; }
        public bool IsActive { get; private set; }

        private Section()
        {
            // Required by EF Core
        }

        public Section(Guid eventId,string name,string? description,int displayOrder)
        {
            EventId = eventId;
            Name = name;
            Description = description;
            DisplayOrder = displayOrder;
            IsActive = true;
        }

        public void Update(string name,string? description,int displayOrder)
        {
            Name = name;
            Description = description;
            DisplayOrder = displayOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
