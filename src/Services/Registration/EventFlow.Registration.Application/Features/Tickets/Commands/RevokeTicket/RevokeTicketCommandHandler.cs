using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Tickets.Commands.RevokeTicket;

public sealed class RevokeTicketCommandHandler(
    ITicketRepository tickets,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<RevokeTicketCommand, TicketDto>
{
    public async Task<TicketDto> Handle(
        RevokeTicketCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var ticket = await tickets.GetByIdAsync(
            command.EventId,
            command.TicketId,
            cancellationToken);

        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found.");
        }

        ticket.IsActive = false;
        ticket.RevokedAtUtc = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.ToDto();
    }
}
