using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using MediatR;
namespace EventFlow.Event.Application.Features.Sponsors.Queries.GetSponsorById;
public sealed class GetSponsorByIdQueryHandler(ISponsorRepository repository, IEventFeatureRepository featureRepository) : IRequestHandler<GetSponsorByIdQuery, GetSponsorByIdResponse?>
{
    public async Task<GetSponsorByIdResponse?> Handle(GetSponsorByIdQuery request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Sponsors, "sponsors", cancellationToken);
        var sponsor = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (sponsor is null || sponsor.EventId != request.EventId) return null;
        return new GetSponsorByIdResponse(sponsor.Id, sponsor.EventId, sponsor.Name, sponsor.Description, sponsor.WebsiteUrl, sponsor.LogoUrl, sponsor.SponsorLevel, sponsor.DisplayOrder, sponsor.IsActive);
    }
}
