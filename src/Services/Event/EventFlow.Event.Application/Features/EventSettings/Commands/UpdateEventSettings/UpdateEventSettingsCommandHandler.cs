
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
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

            if (settings is null)
            {
                // Create the defaults first so the very first update does not fail
                settings = new EventFlow.Event.Domain.Entities.EventSettings(request.EventId);
                await settingsRepository.AddAsync(settings, cancellationToken);
            }

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
