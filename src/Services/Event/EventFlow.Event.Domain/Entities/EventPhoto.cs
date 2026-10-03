using EventFlow.SharedKernel.Domain;

namespace EventFlow.Event.Domain.Entities
{
    public class EventPhoto : Entity
    {
        private EventPhoto()
        {
        }

        public EventPhoto(Guid eventId, Guid photographerId, string imageUrl, string publicId, string? thumbnailUrl = null)
        {
            EventId = eventId;
            PhotographerId = photographerId;
            ImageUrl = imageUrl;
            PublicId = publicId;
            ThumbnailUrl = thumbnailUrl;
            IsVisible = false; // Requires organizer approval
            UploadedAt = DateTime.UtcNow;
        }

        public Guid EventId { get; private set; }
        public Guid PhotographerId { get; private set; }
        public string ImageUrl { get; private set; } = string.Empty;
        public string PublicId { get; private set; } = string.Empty;
        public string? ThumbnailUrl { get; private set; }
        public bool IsVisible { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public Guid? ApprovedBy { get; private set; }
        public string? FaceEmbeddingsJson { get; private set; }

        public void Approve(Guid approvedBy)
        {
            IsVisible = true;
            ApprovedAt = DateTime.UtcNow;
            ApprovedBy = approvedBy;
            SetUpdatedAt();
        }

        public void Hide(Guid hiddenBy)
        {
            IsVisible = false;
            ApprovedAt = null;
            ApprovedBy = hiddenBy;
            SetUpdatedAt();
        }

        public void UpdateFaceEmbeddings(string faceEmbeddingsJson)
        {
            FaceEmbeddingsJson = faceEmbeddingsJson;
            SetUpdatedAt();
        }
    }
}