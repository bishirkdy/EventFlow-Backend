using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Storage;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Exceptions;
using MediatR;
namespace EventFlow.Event.Application.Features.Sponsors.Commands.UpdateSponsor;
public sealed class UpdateSponsorCommandHandler(ISponsorRepository repository, IEventFeatureRepository featureRepository, IFileStorage fileStorage, IUnitOfWork unitOfWork) : IRequestHandler<UpdateSponsorCommand>
{
    public async Task Handle(UpdateSponsorCommand request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Sponsors, "sponsors", cancellationToken);
        var sponsor = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (sponsor is null || sponsor.EventId != request.EventId) throw new NotFoundException("Sponsor not found.");
        var logoUrl = sponsor.LogoUrl;
        if (request.Logo is not null) logoUrl = (await fileStorage.SaveAsync(request.Logo, $"events/{request.EventId:D}/sponsors", cancellationToken)).Url;
        sponsor.Update(request.Name.Trim(), request.Description, request.WebsiteUrl, logoUrl, request.SponsorLevel.Trim(), request.DisplayOrder, request.IsActive);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
