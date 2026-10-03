using MediatR;

namespace EventFlow.Event.Application.Features.Commands.UpdateEventPhotoVisibility
{
    public sealed record UpdateEventPhotoVisibilityCommand(Guid PhotoId, bool IsVisible, Guid ApprovedBy) : IRequest;
}