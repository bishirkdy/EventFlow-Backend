using EventFlow.Event.Application.Abstractions.Storage;
using MediatR;

namespace EventFlow.Event.Application.Features.Commands.UploadEventPhoto;

public sealed record UploadEventPhotoCommand(Guid EventId, UploadedFile? File)
    : IRequest<UploadEventPhotoResponse>;
