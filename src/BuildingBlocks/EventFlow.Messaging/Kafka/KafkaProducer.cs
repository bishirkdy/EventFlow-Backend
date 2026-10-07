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

    public async Task PublishAsync<T>(string topic,T message, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        var json = JsonSerializer.Serialize(message);

        await _producer.ProduceAsync(topic,
            new Message<string, string>
            {
                Key = message.EventContextId.ToString(),
                Value = json
            },
            cancellationToken);
    }
}
