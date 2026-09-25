using EventFlow.Event.Application.Abstractions.Storage;
using MediatR;

namespace EventFlow.Event.Application.Features.Speakers.Commands.CreateSpeaker;

public sealed record CreateSpeakerCommand(Guid EventId, string Name, string? Bio, string? Designation, string? Organization, string? Email, int DisplayOrder, UploadedFile? Image) : IRequest<Guid>;
