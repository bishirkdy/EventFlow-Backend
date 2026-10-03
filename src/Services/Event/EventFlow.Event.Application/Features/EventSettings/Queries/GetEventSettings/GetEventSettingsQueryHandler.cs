using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using MediatR;


namespace EventFlow.Event.Application.Features.EventSettings.Queries.GetEventSettings
{
    // Query Handler
    public sealed class GetEventSettingsQueryHandler(
        IEventSettingsRepository settingsRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : IRequestHandler<GetEventSettingsQuery,GetEventSettingsResponse?>
    {
        public async Task<GetEventSettingsResponse?> Handle(GetEventSettingsQuery request,CancellationToken cancellationToken)
        {
            // Get event settings
            var settings = await settingsRepository.GetByEventIdAsync(request.EventId,cancellationToken);

            if (settings is null)
            {
                // Settings rows are optional, so create the defaults on first access
                // instead of failing with a 404 for events created before settings existed.
                settings = new EventFlow.Event.Domain.Entities.EventSettings(request.EventId);
                await settingsRepository.AddAsync(settings, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Map entity to response
            return mapper.Map<GetEventSettingsResponse>(settings);
        }
    }
}
