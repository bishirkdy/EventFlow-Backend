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
        bool ShowRegistrationForm,
        string? Configuration) : IRequest<string?>;
}
