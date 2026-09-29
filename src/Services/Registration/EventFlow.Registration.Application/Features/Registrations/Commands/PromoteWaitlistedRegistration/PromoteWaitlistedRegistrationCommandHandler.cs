using EventFlow.Contracts.Common;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.PromoteWaitlistedRegistration;

public sealed class PromoteWaitlistedRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IRegistrationFormRepository forms,
    IUnitOfWork unitOfWork,
    ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<
        PromoteWaitlistedRegistrationCommand,
        ApiResponse<RegistrationDto>>
{
    public async Task<ApiResponse<RegistrationDto>> Handle(
        PromoteWaitlistedRegistrationCommand command,
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

        if (registration.Status != RegistrationStatus.Waitlisted)
        {
            return ApiResponse<RegistrationDto>.Fail(
                ["Registration is not waitlisted."]);
        }

        var form = await forms.GetByEventIdAsync(
            command.EventId,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (form?.CapacityMode == CapacityMode.Limited &&
            form.Capacity.HasValue &&
            await registrations.CountByStatusAsync(
                command.EventId,
                RegistrationStatus.Approved,
                cancellationToken) >= form.Capacity.Value)
        {
            return ApiResponse<RegistrationDto>.Fail(
                ["No available capacity."]);
        }

        var now = DateTime.UtcNow;

        registration.Status = RegistrationStatus.Approved;
        registration.ApprovedAtUtc = now;
        registration.WaitlistedAtUtc = null;
        registration.WaitlistPosition = null;

        if (registration.Participant is not null &&
            registration.Ticket is null)
        {
            var ticketId = Guid.NewGuid();

            registration.Ticket = new Ticket
            {
                Id = ticketId,
                RegistrationId = registration.Id,
                ParticipantId = registration.Participant.Id,
                TicketNumber =
                    $"TKT-{now:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}",
                QrCodeValue =
                    $"eventflow:{command.EventId}:{registration.Participant.Id}:{ticketId}",
                IssuedAtUtc = now
            };
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<RegistrationDto>.Success(
            registration.ToDto(),
            "Waitlisted registration promoted.");
    }
}
