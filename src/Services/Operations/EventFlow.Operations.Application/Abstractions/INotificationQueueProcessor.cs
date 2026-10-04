namespace EventFlow.Operations.Application.Abstractions;

public interface INotificationQueueProcessor
{
    Task<int> ProcessDueAsync(CancellationToken cancellationToken = default);
}
