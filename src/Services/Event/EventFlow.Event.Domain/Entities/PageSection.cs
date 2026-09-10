using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities
{
    public sealed class PageSection : Entity
    {
        public Guid PageId { get; private set; }
        public string SectionType { get; private set; } = string.Empty;
        public string? Title { get; private set; }
        public string? Content { get; private set; }
        public string? ImageUrl { get; private set; }
        public int DisplayOrder { get; private set; }
        public bool IsVisible { get; private set; }
        public string? Configuration { get; private set; }

        private PageSection()
        {
            // Required by EF Core
        }

        public PageSection(Guid pageId,string sectionType,string? title,string? content,string? imageUrl,int displayOrder,string? configuration)
        {
            PageId = pageId;
            SectionType = sectionType;
            Title = title;
            Content = content;
            ImageUrl = imageUrl;
            DisplayOrder = displayOrder;
            IsVisible = true;
            Configuration = configuration;
        }

        public void Update(string sectionType,string? title,string? content,string? imageUrl,int displayOrder,string? configuration)
        {
            SectionType = sectionType;
            Title = title;
            Content = content;
            ImageUrl = imageUrl;
            DisplayOrder = displayOrder;
            Configuration = configuration;
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
