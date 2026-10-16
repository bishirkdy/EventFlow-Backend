using System.Text.Json;
using EventFlow.Messaging.Events;
using EventFlow.Messaging.Kafka;

namespace EventFlow.Operations.Infrastructure.Tests;

// In-memory producer that records published messages and can be told to fail a
// number of times to simulate an unavailable broker.
public sealed class FakeKafkaProducer : IKafkaProducer
{
    private readonly object _gate = new();
    private int _failures;

    public List<(string Topic, string Key, string Payload)> Published { get; } = new();

    public int FailTimes { get; set; }

    public Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default)
        where T : IntegrationEvent
    {
        return PublishAsync(topic, message.EventContextId.ToString(), JsonSerializer.Serialize(message), cancellationToken);
    }

    public Task PublishAsync(string topic, string key, string payload, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            if (_failures < FailTimes)
            {
                _failures++;
                throw new InvalidOperationException("Kafka is unavailable.");
            }

            Published.Add((topic, key, payload));
        }

        return Task.CompletedTask;
    }
}
