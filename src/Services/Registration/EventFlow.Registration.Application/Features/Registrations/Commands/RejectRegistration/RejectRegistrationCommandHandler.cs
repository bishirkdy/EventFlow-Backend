using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Enums;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.RejectRegistration;

public sealed class RejectRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<RejectRegistrationCommand, ApiResponse<RegistrationDto>>
{
    public async Task<ApiResponse<RegistrationDto>> Handle(
        RejectRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            return ApiResponse<RegistrationDto>.Fail(
                ["You do not have permission."]);
        }

        var registration = await registrations.GetByIdAsync(
            command.EventId,
            command.RegistrationId,
            includeParticipant: true,
            includeTicket: true,
            cancellationToken: cancellationToken);

        if (registration is null)
        {
            return ApiResponse<RegistrationDto>.Fail(
                ["Registration not found."]);
        }

        if (registration.Status == RegistrationStatus.Cancelled)
        {
            return ApiResponse<RegistrationDto>.Fail(
                ["Cancelled registration cannot be rejected."]);
        }

        var now = DateTime.UtcNow;

        registration.Status = RegistrationStatus.Rejected;
        registration.RejectionReason = command.Request.Reason.Trim();
        registration.RejectedAtUtc = now;
        registration.WaitlistPosition = null;
        registration.WaitlistedAtUtc = null;

        if (registration.Ticket is not null)
        {
            registration.Ticket.IsActive = false;
            registration.Ticket.RevokedAtUtc = now;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<RegistrationDto>.Success(
            registration.ToDto(),
            "Registration rejected.");
    }
}
