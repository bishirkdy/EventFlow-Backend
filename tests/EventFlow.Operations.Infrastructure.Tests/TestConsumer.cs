using EventFlow.Messaging.Kafka;
using EventFlow.Operations.Application.Abstractions;
using EventFlow.Operations.Infrastructure.Background;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventFlow.Operations.Infrastructure.Tests;

// Test consumer whose handling logic is supplied by the test.
public sealed class TestConsumer(
    IServiceScopeFactory scopeFactory,
    IOptions<KafkaOptions> kafkaOptions,
    IKafkaProducer producer,
    ILoggerFactory loggerFactory,
    Func<TestIntegrationEvent, IOperationsDbContext, CancellationToken, Task> handler)
    : KafkaConsumerBase<TestIntegrationEvent>(scopeFactory, kafkaOptions, producer, loggerFactory)
{
    protected override string Topic => "test-topic";

    protected override string GroupId => "test-group";

    protected override string ConsumerName => "test-consumer";

    protected override Task HandleMessageAsync(
        TestIntegrationEvent message,
        IOperationsDbContext db,
        CancellationToken cancellationToken)
    {
        return handler(message, db, cancellationToken);
    }
}
