using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities
{
    public sealed class EventPage : Entity
    {
        public Guid EventId { get; private set; }

        public string Name { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public string PageType { get; private set; } = string.Empty;
        public int DisplayOrder { get; private set; }
        public bool IsPublished { get; private set; }

        private EventPage()
        {
            // Required by EF Core
        }

        public EventPage(Guid eventId,string name,string slug,string pageType,int displayOrder)
        {
            EventId = eventId;
            Name = name;
            Slug = slug;
            PageType = pageType;
            DisplayOrder = displayOrder;
            IsPublished = false;
        }

        public void Update(string name,string slug,string pageType,int displayOrder)
        {
            Name = name;
            Slug = slug;
            PageType = pageType;
            DisplayOrder = displayOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            IsPublished = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unpublish()
        {
            IsPublished = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
