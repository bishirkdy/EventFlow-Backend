using EventFlow.Event.Application.Abstractions.Persistence;
using EventFlow.Event.Application.Abstractions.Services;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Event.Application.Features.Events.Commands.ClaimEventOwner;

public sealed class ClaimEventOwnerCommandHandler(
    IEventRepository eventRepository,
    IUserDirectoryClient userDirectoryClient)
    : IRequestHandler<ClaimEventOwnerCommand>
{
    public async Task Handle(
        ClaimEventOwnerCommand request,
        CancellationToken cancellationToken)
    {
        var eventEntity = await eventRepository.GetByIdAsync(
            request.EventId,
            cancellationToken);

        if (eventEntity is null)
        {
            throw new NotFoundException("Event not found.");
        }

        if (eventEntity.CreatedBy != request.UserId)
        {
            throw new ForbiddenException("Only the event creator can claim ownership.");
        }

        await userDirectoryClient.AssignOwnerAsync(
            request.UserId,
            request.EventId,
            cancellationToken);
    }
}
