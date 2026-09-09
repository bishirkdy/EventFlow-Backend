using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Domain.Entities;
using MediatR;

namespace EventFlow.Event.Application.Features.Sessions.Commands.CreateSession
{
    public sealed class CreateSessionCommandHandler(ISessionRepository sessionRepository,IUnitOfWork unitOfWork)
        : IRequestHandler<CreateSessionCommand, Guid>
    {
        public async Task<Guid> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            // Create session
            var session = new Session(
                request.EventId,
                request.SectionId,
                request.Title,
                request.Description,
                request.SessionType,
                request.Capacity);

            // Save session
            await sessionRepository.AddAsync(session,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return session.Id;
        }
    }
}
