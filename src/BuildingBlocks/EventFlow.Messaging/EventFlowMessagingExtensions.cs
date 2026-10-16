using EventFlow.Messaging.Correlation;
using EventFlow.Messaging.Kafka;
using EventFlow.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace EventFlow.Messaging
{
    public static class EventFlowMessagingExtensions
    {
        public static IServiceCollection AddEventFlowMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaOptions>(configuration.GetSection("Kafka"));
            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            services.TryAddSingleton<ICorrelationIdProvider, NullCorrelationIdProvider>();
            return services;
        }

        public static IServiceCollection AddOutbox<TContext>(this IServiceCollection services, IConfiguration configuration)
            where TContext : DbContext, IOutboxDbContext
        {
            services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));
            services.AddScoped<IOutbox, Outbox<TContext>>();
            services.AddSingleton<IHostedService, OutboxPublisher<TContext>>();
            return services;
        }
    }
}
