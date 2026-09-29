using EventFlow.Messaging.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace EventFlow.Messaging
{
    public static class EventFlowMessagingExtensions
    {
        public static IServiceCollection AddEventFlowMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaOptions>(configuration.GetSection("Kafka"));
            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            return services;
        }
    }
}
