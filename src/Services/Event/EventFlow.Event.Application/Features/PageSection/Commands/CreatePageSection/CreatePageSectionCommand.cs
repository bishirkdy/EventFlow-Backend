using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Commands.CreatePageSection
{
    public sealed record CreatePageSectionCommand(
        Guid PageId,
        string SectionType,
        string? Title,
        string? Content,
        string? ImageUrl,
        int DisplayOrder,
        string? Configuration)
        : IRequest<Guid>;
}
