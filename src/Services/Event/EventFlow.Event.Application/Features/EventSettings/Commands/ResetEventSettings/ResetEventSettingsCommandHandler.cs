
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;

namespace EventFlow.Event.Application.Features.EventSettings.Commands.ResetEventSettings
{
    // Command Handler
    public sealed class ResetEventSettingsCommandHandler(IEventSettingsRepository settingsRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<ResetEventSettingsCommand>
    {
        public async Task Handle(ResetEventSettingsCommand request, CancellationToken cancellationToken)
        {
            // Get settings for the event
            var settings = await settingsRepository.GetByEventIdAsync(request.EventId, cancellationToken);

            // Check whether settings exist
            if (settings is null)
                throw new KeyNotFoundException("Event settings not found.");

            // Restore default settings
            settings.Reset();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
