using EventFlow.SharedKernel.Domain;

namespace EventFlow.Event.Domain.Entities
{
    public sealed class NavigationItem : Entity
    {
        public string Label { get; private set; } = string.Empty;
        public Guid PageId { get; private set; }
        public int DisplayOrder { get; private set; }
        public bool IsVisible { get; private set; }
        public Guid EventId { get; private set; }

        private NavigationItem()
        {
            // Required by EF Core
        }

        public NavigationItem(
            Guid eventId,
            string label,
            Guid pageId,
            int displayOrder)
        {
            Id = Guid.NewGuid();
            EventId = eventId;
            Label = label;
            PageId = pageId;
            DisplayOrder = displayOrder;
            IsVisible = true;
        }

        public void Update(string label, Guid pageId)
        {
            Label = label;
            PageId = pageId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetVisibility(bool isVisible)
        {
            IsVisible = isVisible;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDisplayOrder(int displayOrder)
        {
            DisplayOrder = displayOrder;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
