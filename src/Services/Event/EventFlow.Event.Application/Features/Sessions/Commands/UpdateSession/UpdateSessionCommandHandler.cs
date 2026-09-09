using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Commands.UpdateSession
{
    public sealed class UpdateSessionCommandHandler(ISessionRepository sessionRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateSessionCommand>
    {
        public async Task Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
        {
            // Get session
            var session = await sessionRepository.GetByIdAsync(request.Id, cancellationToken);

            // Verify session belongs to event
            if (session is null || session.EventId != request.EventId)
                throw new NotFoundException("Session not found.");

            // Update session
            session.Update(
                request.Title,
                request.Description,
                request.SessionType,
                request.Capacity);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
