using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.Event.Application.Exceptions;
using MediatR;
namespace EventFlow.Event.Application.Features.Sponsors.Commands.DeleteSponsor;
public sealed class DeleteSponsorCommandHandler(ISponsorRepository repository, IEventFeatureRepository featureRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteSponsorCommand>
{
    public async Task Handle(DeleteSponsorCommand request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Sponsors, "sponsors", cancellationToken);
        var sponsor = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (sponsor is null || sponsor.EventId != request.EventId) throw new NotFoundException("Sponsor not found.");
        repository.Remove(sponsor);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
