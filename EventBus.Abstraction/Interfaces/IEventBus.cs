using SharedCollection.AbstractClass;
using SharedCollection.Interfaces;

namespace EventBus.Abstraction.Interfaces
{
    public interface IEventBus
    {
        Task Publish(IntegrationEvent @event);

        Task Subscribe<T, TH>()
            where T : class, IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        Task Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : class, IIntegrationEvent;
    }
}
