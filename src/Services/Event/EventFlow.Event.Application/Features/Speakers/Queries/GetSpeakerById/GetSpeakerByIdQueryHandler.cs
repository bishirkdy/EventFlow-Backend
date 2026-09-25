using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Exceptions;
using MediatR;
namespace EventFlow.Event.Application.Features.Speakers.Queries.GetSpeakerById;
public sealed class GetSpeakerByIdQueryHandler(ISpeakerRepository repository, IEventFeatureRepository featureRepository) : IRequestHandler<GetSpeakerByIdQuery, GetSpeakerByIdResponse?>
{
    public async Task<GetSpeakerByIdResponse?> Handle(GetSpeakerByIdQuery request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Speakers, "speakers", cancellationToken);
        var speaker = await repository.GetByIdWithSessionsAsync(request.Id, cancellationToken);
        if (speaker is null || speaker.EventId != request.EventId) return null;
        var sessions = await repository.GetSessionsAsync(speaker.Id, cancellationToken);
        return new GetSpeakerByIdResponse(speaker.Id, speaker.EventId, speaker.Name, speaker.Bio, speaker.Designation, speaker.Organization, speaker.Email, speaker.ImageUrl, speaker.DisplayOrder, speaker.IsActive, sessions.Select(x => new SpeakerSessionResponse(x.Id, x.Title, x.SessionType, x.StartTimeUtc, x.EndTimeUtc)).ToList());
    }
}
