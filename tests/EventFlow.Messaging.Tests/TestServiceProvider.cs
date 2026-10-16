using EventFlow.Messaging.Correlation;
using EventFlow.Messaging.Kafka;
using EventFlow.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Messaging.Tests;

internal static class TestServiceProvider
{
    public static ServiceProvider Build(string databaseName, IKafkaProducer producer)
    {
        var configuration = new ConfigurationBuilder().Build();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddDbContext<TestOutboxDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));
        services.AddSingleton(producer);
        services.AddSingleton<ICorrelationIdProvider, NullCorrelationIdProvider>();
        services.AddOutbox<TestOutboxDbContext>(configuration);

        return services.BuildServiceProvider();
    }
}
