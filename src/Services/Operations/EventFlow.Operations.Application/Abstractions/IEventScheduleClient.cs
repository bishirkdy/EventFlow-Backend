namespace EventFlow.Operations.Application.Abstractions;
public interface IEventScheduleClient { Task<int> GetSessionCountAsync(Guid eventId,CancellationToken cancellationToken=default); }
