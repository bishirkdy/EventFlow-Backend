
using MediatR;

namespace EventFlow.Event.Application.Features.EventSettings.Commands.UpdateEventSettings
{
    // Command
    public sealed record UpdateEventSettingsCommand(
        Guid EventId,
        bool RegistrationEnabled,
        bool AttendanceEnabled,
        bool FeedbackEnabled,
        bool CertificateEnabled,
        bool GalleryEnabled,
        string DefaultLanguage) : IRequest;
}
