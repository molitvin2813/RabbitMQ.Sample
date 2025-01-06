using EventBus.Abstraction.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedCollection.AbstractClass;
using SharedCollection.Enums;
using SharedCollection.Events;
using SharedCollection.Exceptions;

namespace CatalogService.PostgreSQL.Services
{
    public class IntegrationEventOutboxService
        : IIntegrationEventOutboxService
    {
        public IntegrationEventOutboxService(CatalogServiceContext context)
        {
            _context = context;
        }

        private readonly CatalogServiceContext _context;

        public async Task<bool> CheckEntryEvent(Guid integrationEventId, CancellationToken token = default)
        {
            var data = await _context.Events
                .AsNoTracking()
                .Where(x => x.EventOutboxId == integrationEventId)
                .Select(x => x.EventOutboxId)
                .FirstOrDefaultAsync(token);

            return data != Guid.Empty;
        }

        public async Task<Dictionary<Guid, IntegrationEvent>> GetUnsentEvents(int take, CancellationToken token = default)
        {
            Dictionary<Guid, IntegrationEvent> result = new Dictionary<Guid, IntegrationEvent>();

            var data = await _context.Events
                .AsNoTracking()
                .Where(x => x.PublishStatus == EventPublishStatusEnum.Unpublished)
                .OrderBy(x => x.CreateDate)
                .Take(take)
                .ToListAsync(token);

            foreach (var item in data)
            {
                IntegrationEvent temp = JsonConvert.DeserializeObject(item.EventDTO, Type.GetType(item.EventType)) as IntegrationEvent;
                result.Add(item.EventOutboxId, temp);
            }

            return result;
        }

        public async Task<Dictionary<Guid, IntegrationEvent>> GetUnprocessedEvents(CancellationToken token = default)
        {
            Dictionary<Guid, IntegrationEvent> result = new Dictionary<Guid, IntegrationEvent>();

            var data = await _context.Events
                .AsNoTracking()
                .Where(x =>
                    x.ProcessingStatus == EventProcessingStatusEnum.Income ||
                    x.ProcessingStatus == EventProcessingStatusEnum.InProgress
                )
                .OrderBy(x => x.CreateDate)
                .ToListAsync(token);

            foreach (var item in data)
            {
                IntegrationEvent temp = JsonConvert.DeserializeObject(item.EventDTO, Type.GetType(item.EventType)) as IntegrationEvent;
                result.Add(item.EventOutboxId, temp);
            }

            return result;
        }

        public async Task ChangeEventPublishStatusAsync(
            Guid integrationEventId,
            string? messageError,
            EventPublishStatusEnum status,
            CancellationToken token = default)
        {

            var integrationEvent = await _context.Events
                .FirstOrDefaultAsync(x => x.EventOutboxId == integrationEventId, token);

            if (integrationEvent == null)
            {
                throw new RecordNotFoundException<Guid>(nameof(EventOutbox), integrationEventId);
            }

            integrationEvent.PublishStatus = status;
        }

        public async Task ChangeEventProcessingStatusAsync(
           Guid integrationEventId,
           string? messageError,
           EventProcessingStatusEnum status,
           CancellationToken token = default)
        {

            var integrationEvent = await _context.Events
                .FirstOrDefaultAsync(x => x.EventOutboxId == integrationEventId, token);

            if (integrationEvent == null)
            {
                throw new RecordNotFoundException<Guid>(nameof(EventOutbox), integrationEventId);
            }

            integrationEvent.ProcessingStatus = status;
        }

        public async Task SaveEventAsync(
            IntegrationEvent integrationEvent,
            bool IsIncome,
            CancellationToken token = default)
        {
            var data = new EventOutbox()
            {
                EventDTO = JsonConvert.SerializeObject(integrationEvent),
                EventType = integrationEvent.GetType().AssemblyQualifiedName,
                PublishStatus = IsIncome ? null : EventPublishStatusEnum.Unpublished,
                ProcessingStatus = IsIncome ? EventProcessingStatusEnum.Income : null,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                IsIncomeEvent = IsIncome,
                EventOutboxId = integrationEvent.IntegrationEventId,
            };

            await _context.Events.AddAsync(data, token);
        }
    }
}
