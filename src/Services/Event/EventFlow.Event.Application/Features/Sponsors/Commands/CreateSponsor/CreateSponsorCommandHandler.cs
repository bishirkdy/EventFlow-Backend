using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Domain.Entities;
using MediatR;
namespace EventFlow.Event.Application.Features.Sponsors.Commands.CreateSponsor;
public sealed class CreateSponsorCommandHandler(ISponsorRepository repository, IEventFeatureRepository featureRepository, IFileStorage fileStorage, IUnitOfWork unitOfWork) : IRequestHandler<CreateSponsorCommand, Guid>
{
    public async Task<Guid> Handle(CreateSponsorCommand request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Sponsors, "sponsors", cancellationToken);
        string? logoUrl = null;
        if (request.Logo is not null) logoUrl = (await fileStorage.SaveAsync(request.Logo, $"events/{request.EventId:D}/sponsors", cancellationToken)).Url;
        var sponsor = new Sponsor(request.EventId, request.Name.Trim(), request.Description, request.WebsiteUrl, logoUrl, request.SponsorLevel.Trim(), request.DisplayOrder);
        await repository.AddAsync(sponsor, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return sponsor.Id;
    }
}
