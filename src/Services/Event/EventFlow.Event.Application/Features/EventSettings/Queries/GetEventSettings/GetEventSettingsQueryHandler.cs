using AutoMapper;
using EventFlow.Event.Application.Abstractions.Persistence;
using MediatR;


namespace EventFlow.Event.Application.Features.EventSettings.Queries.GetEventSettings
{
    // Query Handler
    public sealed class GetEventSettingsQueryHandler(IEventSettingsRepository settingsRepository,IMapper mapper)
        : IRequestHandler<GetEventSettingsQuery,GetEventSettingsResponse?>
    {
        public async Task<GetEventSettingsResponse?> Handle(GetEventSettingsQuery request,CancellationToken cancellationToken)
        {
            // Get event settings
            var settings = await settingsRepository.GetByEventIdAsync(request.EventId,cancellationToken);

            // Return null when settings do not exist
            if (settings is null)
                return null;

            // Map entity to response
            return mapper.Map<GetEventSettingsResponse>(settings);
        }
    }
}
