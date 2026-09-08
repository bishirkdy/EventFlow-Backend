

using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventSettings.Commands.UpdateEventSettings
{
    // Command Handler
    public sealed class UpdateEventSettingsCommandHandler(IEventSettingsRepository settingsRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateEventSettingsCommand>
    {
        public async Task Handle(UpdateEventSettingsCommand request, CancellationToken cancellationToken)
        {
            // Get settings for the event
            var settings = await settingsRepository.GetByEventIdAsync(request.EventId,cancellationToken);

            // Check whether settings exist
            if (settings is null)
                throw new KeyNotFoundException("Event settings not found.");

            // Update settings through the domain method
            settings.Update(
                request.RegistrationEnabled,
                request.AttendanceEnabled,
                request.FeedbackEnabled,
                request.CertificateEnabled,
                request.GalleryEnabled,
                request.DefaultLanguage);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
