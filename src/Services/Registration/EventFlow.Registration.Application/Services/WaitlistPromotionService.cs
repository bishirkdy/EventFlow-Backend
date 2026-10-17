using EventFlow.Registration.Application.Abstractions.Persistence;

using RegistrationEntity = EventFlow.Registration.Domain.Entities.Registration;

namespace EventFlow.Registration.Application.Services;

public sealed class WaitlistPromotionService(IRegistrationRepository registrations,IRegistrationFormRepository forms,IUnitOfWork unitOfWork)
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
            await unitOfWork.SaveChangesAsync(cancellationToken);
            promoted++;
        }

        return promoted;
    }

    private void Approve(
        RegistrationEntity registration)
    {
        registration.Approve();

        if (registration.Participant is not null &&
            registration.Ticket is null)
        {
            var ticket = Ticket.Issue(
                registration.Id,
                registration.Participant.Id,
                registration.ScopeType,
                registration.ScopeId);

            registration.AttachTicket(ticket);
            registrations.AddTicket(ticket);
        }
    }
}
