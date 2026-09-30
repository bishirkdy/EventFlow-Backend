
namespace EventFlow.SharedKernel.Domain
{
    public interface IDomainEvent
    {
        DateTime OccurredOnUtc { get; }
    }
}
