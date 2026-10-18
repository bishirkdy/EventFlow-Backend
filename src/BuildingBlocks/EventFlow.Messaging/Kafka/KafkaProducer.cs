using Confluent.Kafka;
using EventFlow.Messaging.Events;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventFlow.Messaging.Kafka;

public sealed class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly KafkaOptions _options;
    private readonly object _rebuildGate = new();

    private IProducer<string, string> _producer;

    public KafkaProducer(IOptions<KafkaOptions> options)
    {
        _options = options.Value;
        _producer = Build();
    }

    public Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        return PublishAsync(
            topic,
            message.EventContextId.ToString(),
            JsonSerializer.Serialize(message),
            cancellationToken);
    }

    public async Task PublishAsync(string topic, string key, string payload, CancellationToken cancellationToken = default)
    {
        try
        {
            await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = payload
            },
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // A broker restart (or a DNS blip) can leave the librdkafka handle
            // unable to recover, which shows up as every delivery timing out.
            // Rebuild it so the next attempt gets a fresh connection.
            Rebuild();

            throw;
        }
    }

    private IProducer<string, string> Build()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            MessageTimeoutMs = _options.ProducerMessageTimeoutMs,
            SocketTimeoutMs = _options.ProducerSocketTimeoutMs,
            RequestTimeoutMs = _options.ProducerRequestTimeoutMs
        };

        return new ProducerBuilder<string, string>(config).Build();
    }

    private void Rebuild()
    {
        lock (_rebuildGate)
        {
            try
            {
                _producer.Dispose();
            }
            catch
            {
                // Ignore disposal failures; the client is being replaced anyway.
            }

            _producer = Build();
        }
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}
