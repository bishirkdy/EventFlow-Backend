using MediatR;
namespace EventFlow.Event.Application.Features.Sponsors.Commands.DeleteSponsor;
public sealed record DeleteSponsorCommand(Guid Id, Guid EventId) : IRequest;
