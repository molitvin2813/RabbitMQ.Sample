using EventBus.Abstraction.Interfaces;
using SharedCollection.AbstractClass;
using SharedCollection.Interfaces;

namespace EventBus.RabbitMQ.Interfaces
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        void Clear();

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetTypeEventByName(string eventName);

        void Subscribe<T, TH>()
             where T : class, IIntegrationEvent
             where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>()
             where T : class, IIntegrationEvent
             where TH : IIntegrationEventHandler<T>;

        IEnumerable<Type> GetHandlersForEvent<T>(T integrationEvent) where T : IntegrationEvent;

    }
}