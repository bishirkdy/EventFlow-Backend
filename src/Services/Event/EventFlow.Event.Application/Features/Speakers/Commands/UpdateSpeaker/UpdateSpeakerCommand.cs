using EventFlow.Event.Application.Abstractions.Storage;
using MediatR;

namespace EventFlow.Event.Application.Features.Speakers.Commands.UpdateSpeaker;

public sealed record UpdateSpeakerCommand(Guid Id, Guid EventId, string Name, string? Bio, string? Designation, string? Organization, string? Email, int DisplayOrder, bool IsActive, UploadedFile? Image) : IRequest;
