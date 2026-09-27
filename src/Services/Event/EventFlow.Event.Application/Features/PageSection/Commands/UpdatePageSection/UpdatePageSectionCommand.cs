using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Commands.UpdatePageSection
{
    public sealed record UpdatePageSectionCommand(
        Guid Id,
        Guid PageId,
        string SectionType,
        string? Title,
        string? Content,
        string? ImageUrl,
        string? ImagePublicId,
        bool IsVisible,
        string? Configuration) : IRequest<string?>;
}
