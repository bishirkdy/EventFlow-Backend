

using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Commands.UpdatePageSection
{
    public sealed record UpdatePageSectionCommand(
        Guid PageId,
        Guid Id,
        string SectionType,
        string? Title,
        string? Content,
        string? ImageUrl,
        int DisplayOrder,
        bool IsVisible,
        string? Configuration)
        : IRequest;
}
