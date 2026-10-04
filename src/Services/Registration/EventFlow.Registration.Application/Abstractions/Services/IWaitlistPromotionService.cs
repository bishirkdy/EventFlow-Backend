namespace EventFlow.Registration.Application.Abstractions.Services;

public interface IWaitlistPromotionService
{
    Task<int> PromoteWaitlistedUntilCapacityAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);
}
