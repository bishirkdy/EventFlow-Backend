using MediatR;

namespace EventFlow.Event.Application.Features.Commands.DeleteEventPhoto
{
    public sealed record DeleteEventPhotoCommand(Guid PhotoId, Guid RequestedBy) : IRequest;
}