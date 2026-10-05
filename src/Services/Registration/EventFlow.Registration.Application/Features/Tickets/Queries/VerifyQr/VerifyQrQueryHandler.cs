using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Tickets.Queries.VerifyQr;

public sealed class VerifyQrQueryHandler(ITicketRepository tickets,EventFlow.Security.Authentication.ICurrentUserService user,IEventRegistrationAccessService access)
    : IRequestHandler<VerifyQrQuery, TicketDto>
{
    public async Task<TicketDto> Handle(VerifyQrQuery query, CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(query.EventId,user.UserId, cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var ticket = await tickets.GetByQrCodeAsync(query.EventId,query.QrCodeValue,cancellationToken);

        if (ticket is null)
        {
            throw new NotFoundException("QR code is invalid.");
        }

        if (!ticket.IsActive)
        {
            throw new ConflictException("Ticket is inactive or revoked.");
        }

        return ticket.ToDto();
    }
}
