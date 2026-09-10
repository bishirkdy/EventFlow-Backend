

using MediatR;

namespace EventFlow.Event.Application.Features.PageSection.Commands.DeletePageSection
{
    public sealed record DeletePageSectionCommand(Guid PageId,Guid Id) : IRequest;
}
