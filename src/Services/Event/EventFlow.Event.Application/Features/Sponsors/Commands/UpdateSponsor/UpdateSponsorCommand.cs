using EventFlow.Event.Application.Abstractions.Storage;
using MediatR;
namespace EventFlow.Event.Application.Features.Sponsors.Commands.UpdateSponsor;
public sealed record UpdateSponsorCommand(Guid Id, Guid EventId, string Name, string? Description, string? WebsiteUrl, string SponsorLevel, int DisplayOrder, bool IsActive, UploadedFile? Logo) : IRequest;
