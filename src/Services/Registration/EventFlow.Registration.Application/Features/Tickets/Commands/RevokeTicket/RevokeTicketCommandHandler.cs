using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using MediatR;

namespace EventFlow.Registration.Application.Features.Tickets.Commands.RevokeTicket;

public sealed class RevokeTicketCommandHandler(
    ITicketRepository tickets,
    IUnitOfWork unitOfWork,
    ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<RevokeTicketCommand, ApiResponse<TicketDto>>
{
    public async Task<ApiResponse<TicketDto>> Handle(
        RevokeTicketCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<TicketDto>.Fail(
                ["You do not have permission."]);
        }

        var ticket = await tickets.GetByIdAsync(
            command.EventId,
            command.TicketId,
            cancellationToken);

        if (ticket is null)
        {
            return ApiResponse<TicketDto>.Fail(
                ["Ticket not found."]);
        }

        ticket.IsActive = false;
        ticket.RevokedAtUtc = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<TicketDto>.Success(
            ticket.ToDto(),
            "Ticket revoked.");
    }
}
