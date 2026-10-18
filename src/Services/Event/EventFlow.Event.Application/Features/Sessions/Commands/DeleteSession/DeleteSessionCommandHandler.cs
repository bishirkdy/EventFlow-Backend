using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Common;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Commands.DeleteSession;

public sealed class DeleteSessionCommandHandler(ISessionRepository sessionRepository, IUnitOfWork unitOfWork, IEventFeatureRepository featureRepository) : IRequestHandler<DeleteSessionCommand>
{
    public async Task Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
    {
        await EventFeatureGuard.EnsureEnabledAsync(featureRepository, request.EventId, FeatureIds.Sessions, "sessions", cancellationToken);

        var session = await sessionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (session is null || session.EventId != request.EventId)
            throw new NotFoundException("Session not found.");

        sessionRepository.Remove(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
