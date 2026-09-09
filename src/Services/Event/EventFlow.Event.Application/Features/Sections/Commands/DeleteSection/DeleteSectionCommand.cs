using MediatR;


namespace EventFlow.Event.Application.Features.Sections.Commands.DeleteSection
{
    public sealed record DeleteSectionCommand(Guid Id,Guid EventId) : IRequest;
}
