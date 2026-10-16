using System.Text.Json;
using Confluent.Kafka;
using EventFlow.Messaging.Events;
using Microsoft.Extensions.Options;

namespace EventFlow.Messaging.Kafka;

public sealed class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IOptions<KafkaOptions> options)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
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
        await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = payload
            },
            cancellationToken);
    }
}
