using EventFlow.Messaging.Events;


namespace EventFlow.Messaging.Kafka;

public interface IKafkaProducer
{
    Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : IntegrationEvent;
}