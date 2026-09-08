using EventFlow.Event.Domain.Common;
using EventFlow.Event.Domain.Enums;


namespace EventFlow.Event.Domain.Entities
{
    public sealed class EventEntity : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        
        public string EventType { get; private set; } = string.Empty;
        public string? SubType { get; private set; }
        
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string TimeZone { get; private set; } = string.Empty;

        public EventStatus Status { get; private set; }

        public string? Subdomain { get; private set; }

        public Guid CreatedBy { get; private set; }

        private EventEntity()
        {
            // Required by EF Core
        }

        public EventEntity(string name, string? description, string eventType, string? subType, DateTime startDate, DateTime endDate, string timeZone,Guid createdBy)
        {
            ValidateDates(startDate, endDate);

            Name = name;
            Description = description;
            EventType = eventType;
            SubType = subType;
            StartDate = startDate;
            EndDate = endDate;
            TimeZone = timeZone;
            CreatedBy = createdBy;
            Status = EventStatus.Draft;
        }

        public void Update(string name,string? description,string eventType,string? subType,DateTime startDate,DateTime endDate,string timeZone)
        {
            ValidateDates(startDate, endDate);

            Name = name;
            Description = description;
            EventType = eventType;
            SubType = subType;
            StartDate = startDate;
            EndDate = endDate;
            TimeZone = timeZone;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            if (Status != EventStatus.Draft)
            {
                throw new InvalidOperationException("Only draft events can be published.");
            }

            Status = EventStatus.Published;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == EventStatus.Completed)
            {
                throw new InvalidOperationException("A completed event cannot be cancelled.");
            }

            Status = EventStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        private static void ValidateDates(DateTime startDate, DateTime endDate)
        {
            if (endDate <= startDate)
            {
                throw new ArgumentException("Event end date must be greater than start date.");
            }
        }
    }
}
