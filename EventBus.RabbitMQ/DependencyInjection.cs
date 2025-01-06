using EventBus.Abstraction.Interfaces;
using EventBus.RabbitMQ.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.RabbitMQ
{
    public static class DependencyInjection
    {
        public static void AddEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddSingleton<IRabbitMQPersistentConnection, DefaultRabbitMQPersistentConnection>();
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();

        }
    }
}
