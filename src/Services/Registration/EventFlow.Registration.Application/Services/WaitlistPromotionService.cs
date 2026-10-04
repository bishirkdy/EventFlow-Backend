using System.Security.Cryptography;
using EventFlow.Registration.Application.Abstractions.Persistence;

using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Application.Services;

public sealed class WaitlistPromotionService(
    IRegistrationRepository registrations,
    IRegistrationFormRepository forms,
    IUnitOfWork unitOfWork)
    : IWaitlistPromotionService
{
    public async Task<int> PromoteWaitlistedUntilCapacityAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var form = await forms.GetByEventIdAsync(
            eventId,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (form is not { CapacityMode: CapacityMode.Limited } ||
            !form.Capacity.HasValue)
        {
            return 0;
        }

        var promoted = 0;

        while (
            await registrations.CountByStatusAsync(
                eventId,
                RegistrationStatus.Approved,
                cancellationToken) < form.Capacity.Value)
        {
            var next = await registrations.GetNextWaitlistedAsync(
                eventId,
                cancellationToken);

            if (next is null)
            {
                break;
            }

            Approve(next);
            promoted++;
        }

        if (promoted > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return promoted;
    }

    private static void Approve(
        RegistrationEntity registration)
    {
        var now = DateTime.UtcNow;

        registration.Status = RegistrationStatus.Approved;
        registration.ApprovedAtUtc = now;
        registration.WaitlistedAtUtc = null;
        registration.WaitlistPosition = null;

        if (registration.Participant is not null &&
            registration.Ticket is null)
        {
            registration.Ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                RegistrationId = registration.Id,
                ParticipantId = registration.Participant.Id,
                TicketNumber =
                    $"TKT-{now:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}",
                QrCodeValue =
                    Convert.ToBase64String(
                            RandomNumberGenerator.GetBytes(32))
                        .Replace("+", "-")
                        .Replace("/", "_")
                        .TrimEnd('='),
                IssuedAtUtc = now
            };
        }
    }
}
