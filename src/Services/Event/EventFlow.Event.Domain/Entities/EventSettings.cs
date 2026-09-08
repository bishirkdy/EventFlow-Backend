

using EventFlow.Event.Domain.Common;

namespace EventFlow.Event.Domain.Entities
{
    public sealed class EventSettings : Entity
    {
        public Guid EventId { get; private set; }
        public bool RegistrationEnabled { get; private set; }
        public bool AttendanceEnabled { get; private set; }
        public bool FeedbackEnabled { get; private set; }
        public bool CertificateEnabled { get; private set; }
        public bool GalleryEnabled { get; private set; }
        public string DefaultLanguage { get; private set; } = string.Empty;

        private EventSettings()
        {
            // Required by EF Core
        }

        public EventSettings(Guid eventId)
        {
            EventId = eventId;

            // Default settings
            RegistrationEnabled = true;
            AttendanceEnabled = true;
            FeedbackEnabled = false;
            CertificateEnabled = false;
            GalleryEnabled = false;
            DefaultLanguage = "en";
        }

        public void Update(
            bool registrationEnabled,
            bool attendanceEnabled,
            bool feedbackEnabled,
            bool certificateEnabled,
            bool galleryEnabled,
            string defaultLanguage)
        {
            // Update event settings
            RegistrationEnabled = registrationEnabled;
            AttendanceEnabled = attendanceEnabled;
            FeedbackEnabled = feedbackEnabled;
            CertificateEnabled = certificateEnabled;
            GalleryEnabled = galleryEnabled;
            DefaultLanguage = defaultLanguage;

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
