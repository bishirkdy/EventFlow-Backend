
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
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

            if (settings is null)
            {
                // The defaults are identical to a reset, so just create them
                await settingsRepository.AddAsync(new EventFlow.Event.Domain.Entities.EventSettings(request.EventId), cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            // Restore default settings
            settings.Reset();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
