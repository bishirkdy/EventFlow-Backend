using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities
{
    public sealed class EventImage : Entity
    {
        public Guid EventId { get; private set; }
        public string Url { get; private set; } = string.Empty;
        public string StorageKey { get; private set; } = string.Empty;
        public string OriginalFileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public long SizeBytes { get; private set; }
        public int DisplayOrder { get; private set; }
        public EventEntity Event { get; set; } = null!;
        private EventImage()
        {
            // Required by EF Core
        }

        public EventImage(
            Guid eventId,
            string url,
            string storageKey,
            string originalFileName,
            string contentType,
            long sizeBytes,
            int displayOrder)
        {
            if (eventId == Guid.Empty)
                throw new ArgumentException("Event ID is required.", nameof(eventId));

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image URL is required.", nameof(url));

            if (string.IsNullOrWhiteSpace(storageKey))
                throw new ArgumentException("Image storage key is required.", nameof(storageKey));

            if (string.IsNullOrWhiteSpace(originalFileName))
                throw new ArgumentException("Original image file name is required.", nameof(originalFileName));

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Image content type is required.", nameof(contentType));

            if (sizeBytes <= 0)
                throw new ArgumentException("Image size must be greater than zero.", nameof(sizeBytes));

            if (displayOrder < 0)
                throw new ArgumentOutOfRangeException(nameof(displayOrder), "Display order cannot be negative.");

            EventId = eventId;
            Url = url;
            StorageKey = storageKey;
            OriginalFileName = originalFileName;
            ContentType = contentType;
            SizeBytes = sizeBytes;
            DisplayOrder = displayOrder;
        }

        public void SetDisplayOrder(int displayOrder)
        {
            if (displayOrder < 0)
                throw new ArgumentOutOfRangeException(nameof(displayOrder), "Display order cannot be negative.");

            DisplayOrder = displayOrder;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
