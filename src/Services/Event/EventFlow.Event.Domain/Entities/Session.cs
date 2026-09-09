using EventFlow.Event.Domain.Common;


namespace EventFlow.Event.Domain.Entities
{
    public sealed class Session : Entity
    {
        public Guid EventId { get; private set; }
        public Guid SectionId { get; private set; }

        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string SessionType { get; private set; } = string.Empty;
        public int? Capacity { get; private set; }
        public string Status { get; private set; } = "Draft";

        private Session()
        {
            // Required by EF Core
        }

        public Session(Guid eventId,Guid sectionId,string title,string? description,string sessionType,int? capacity)
        {
            EventId = eventId;
            SectionId = sectionId;
            Title = title;
            Description = description;
            SessionType = sessionType;
            Capacity = capacity;
            Status = "Draft";
        }

        public void Update(string title,string? description,string sessionType,int? capacity)
        {
            Title = title;
            Description = description;
            SessionType = sessionType;
            Capacity = capacity;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
