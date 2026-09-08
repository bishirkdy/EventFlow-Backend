namespace EventFlow.Event.Api.Requests.EventSettings
{
    // API Request
    public sealed record UpdateEventSettingsRequest(
        bool RegistrationEnabled,
        bool AttendanceEnabled,
        bool FeedbackEnabled,
        bool CertificateEnabled,
        bool GalleryEnabled,
        string DefaultLanguage);
}
