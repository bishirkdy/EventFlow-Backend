using System.Security.Cryptography;
using EventFlow.Registration.Application.Abstractions.Persistence;
using EventFlow.Registration.Application.Abstractions.Services;
using EventFlow.Registration.Application.Common.Mappings;
using EventFlow.Registration.Application.Contracts.Registrations;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;
using EventFlow.SharedKernel.Exceptions;
using MediatR;

namespace EventFlow.Registration.Application.Features.Registrations.Commands.ApproveRegistration;

public sealed class ApproveRegistrationCommandHandler(
    IRegistrationRepository registrations,
    IRegistrationFormRepository forms,
    IUnitOfWork unitOfWork,
    EventFlow.Security.Authentication.ICurrentUserService user,
    IEventRegistrationAccessService access)
    : IRequestHandler<ApproveRegistrationCommand, RegistrationDto>
{
    public async Task<RegistrationDto> Handle(
        ApproveRegistrationCommand command,
        CancellationToken cancellationToken)
    {
        if (!await access.CanManageRegistrationAsync(
                command.EventId,
                user.UserId,
                cancellationToken))
        {
            throw new ForbiddenException("You do not have permission.");
        }

        var registration = await registrations.GetByIdAsync(
            command.EventId,
            command.RegistrationId,
            includeParticipant: true,
            includeTicket: true,
            cancellationToken: cancellationToken);

        if (registration is null)
        {
            throw new NotFoundException("Registration not found.");
        }

        if (registration.Status is
            RegistrationStatus.Cancelled or
            RegistrationStatus.Rejected)
        {
            throw new ConflictException("Registration cannot be approved.");
        }

        if (registration.Status == RegistrationStatus.Approved)
        {
            return registration.ToDto();
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
            throw new ConflictException("Event capacity has been reached.");
        }

        var now = DateTime.UtcNow;

        registration.Status = RegistrationStatus.Approved;
        registration.ApprovedAtUtc = now;
        registration.WaitlistPosition = null;
        registration.WaitlistedAtUtc = null;
        registration.UpdatedAtUtc = now;

        if (registration.Participant is not null &&
            registration.Ticket is null)
        {
            registration.Ticket = new Ticket
            {
                RegistrationId = registration.Id,
                ParticipantId = registration.Participant.Id,
                TicketNumber =
                    $"TKT-{now:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}",
                QrCodeValue =
                    Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                        .Replace("+", "-")
                        .Replace("/", "_")
                        .TrimEnd('='),
                IssuedAtUtc = now
            };
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return registration.ToDto();
    }
}
