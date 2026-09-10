
using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities
{
    public sealed class NavigationItem : Entity
    {
        public Guid NavigationMenuId { get; private set; }
        public string Label { get; private set; } = string.Empty;
        public string? Url { get; private set; }
        public Guid? PageId { get; private set; }
        public int DisplayOrder { get; private set; }
        public bool IsVisible { get; private set; }
        public bool OpenInNewTab { get; private set; }

        private NavigationItem()
        {
            // Required by EF Core
        }

        public NavigationItem(Guid navigationMenuId,string label,string? url,Guid? pageId,int displayOrder,bool openInNewTab)
        {
            NavigationMenuId = navigationMenuId;
            Label = label;
            Url = url;
            PageId = pageId;
            DisplayOrder = displayOrder;
            IsVisible = true;
            OpenInNewTab = openInNewTab;
        }

        public void Update(string label,string? url,Guid? pageId,int displayOrder,bool openInNewTab)
        {
            Label = label;
            Url = url;
            PageId = pageId;
            DisplayOrder = displayOrder;
            OpenInNewTab = openInNewTab;
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
