using MediatR;


namespace EventFlow.Event.Application.Features.Sections.Commands.CreateSection
{
    public sealed record CreateSectionCommand(
        Guid EventId,
        string Name,
        string? Description,
        int DisplayOrder) : IRequest<Guid>;
}
