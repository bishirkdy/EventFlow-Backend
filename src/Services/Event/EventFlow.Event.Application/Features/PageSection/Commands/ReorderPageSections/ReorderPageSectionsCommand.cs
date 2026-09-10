

using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Commands.ReorderPageSections
{
    public sealed record ReorderPageSectionsCommand(Guid PageId, IReadOnlyList<Guid> SectionIds) : IRequest;
}
