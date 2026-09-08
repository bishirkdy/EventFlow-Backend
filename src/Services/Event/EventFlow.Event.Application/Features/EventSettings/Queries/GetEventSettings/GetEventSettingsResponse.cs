

namespace EventFlow.Event.Application.Features.EventSettings.Queries.GetEventSettings
{
    // Query Response
    public sealed record GetEventSettingsResponse(
        Guid Id,
        Guid EventId,
        bool RegistrationEnabled,
        bool AttendanceEnabled,
        bool FeedbackEnabled,
        bool CertificateEnabled,
        bool GalleryEnabled,
        string DefaultLanguage,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
