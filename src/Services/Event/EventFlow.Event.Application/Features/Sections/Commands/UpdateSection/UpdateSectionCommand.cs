using MediatR;


namespace EventFlow.Event.Application.Features.Sections.Commands.UpdateSection
{
    public sealed record UpdateSectionCommand(
        Guid Id,
        Guid EventId,
        string Name,
        string? Description,
        int DisplayOrder) : IRequest;
}
