using SharedCollection.AbstractClass;
using SharedCollection.Enums;

namespace EventBus.Abstraction.Interfaces
{
    public interface IIntegrationEventOutboxService
    {
        Task<bool> CheckEntryEvent(Guid integrationEventId, CancellationToken token);
        Task SaveEventAsync(IntegrationEvent integrationEvent, bool IsIncome, CancellationToken token);
        Task<Dictionary<Guid, IntegrationEvent>> GetUnsentEvents(int take, CancellationToken token);

        Task<Dictionary<Guid, IntegrationEvent>> GetUnprocessedEvents(CancellationToken token);

        Task ChangeEventPublishStatusAsync(
            Guid integrationEventId,
            string? messageError,
            EventPublishStatusEnum status,
            CancellationToken token);

        Task ChangeEventProcessingStatusAsync(
            Guid integrationEventId,
            string? messageError,
            EventProcessingStatusEnum status,
            CancellationToken token);
    }
}
