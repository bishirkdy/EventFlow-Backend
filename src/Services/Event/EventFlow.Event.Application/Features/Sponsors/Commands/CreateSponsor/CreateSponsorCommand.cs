using EventFlow.Event.Application.Abstractions.Storage;
using MediatR;
namespace EventFlow.Event.Application.Features.Sponsors.Commands.CreateSponsor;
public sealed record CreateSponsorCommand(Guid EventId, string Name, string? Description, string? WebsiteUrl, string SponsorLevel, int DisplayOrder, UploadedFile? Logo) : IRequest<Guid>;
