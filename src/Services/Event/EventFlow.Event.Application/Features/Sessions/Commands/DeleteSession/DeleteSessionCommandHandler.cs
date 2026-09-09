using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Exceptions;
using MediatR;


namespace EventFlow.Event.Application.Features.Sessions.Commands.DeleteSession
{
    public sealed class DeleteSessionCommandHandler(ISessionRepository sessionRepository,IUnitOfWork unitOfWork): IRequestHandler<DeleteSessionCommand>
    {
        public async Task Handle(DeleteSessionCommand request,CancellationToken cancellationToken)
        {
            // Get session
            var session = await sessionRepository.GetByIdAsync(request.Id,cancellationToken);

            // Verify session belongs to event
            if (session is null || session.EventId != request.EventId)
                throw new NotFoundException("Session not found.");

            // Remove session
            sessionRepository.Remove(session);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
