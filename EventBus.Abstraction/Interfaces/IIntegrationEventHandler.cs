using SharedCollection.Interfaces;

namespace EventBus.Abstraction.Interfaces
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : class, IIntegrationEvent
    {
        Task Handle(TIntegrationEvent integrationEvent, CancellationToken token);
    }

    public interface IIntegrationEventHandler
    {
    }
}
