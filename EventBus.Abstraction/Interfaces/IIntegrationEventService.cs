using SharedCollection.AbstractClass;

namespace EventBus.Abstraction.Interfaces
{
    public interface IIntegrationEventService
    {
        Task SaveEventAsync(IntegrationEvent evt);

        Task PublishThroughEventBusAsync(IntegrationEvent evt);

        Task<string> PublishThroughEventBusAsyncRpc(IntegrationEvent evt);

        Task RepublishEventAsync(Guid id);
    }
}
