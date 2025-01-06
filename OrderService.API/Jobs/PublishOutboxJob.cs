using EventBus.Abstraction.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderService.Application.Interfaces;
using Quartz;
using SharedCollection.AbstractClass;
using SharedCollection.Enums;

namespace OrderService.API.Jobs
{
    public sealed class PublishOutboxJob : IJob
    {
        public PublishOutboxJob(
             ILogger<PublishOutboxJob> logger,
             IServiceProvider service)
        {
            _logger = logger;
            _service = service;
        }

        private readonly ILogger<PublishOutboxJob> _logger;
        private readonly IServiceProvider _service;

        public static readonly JobKey JobKey = new JobKey("PublishOutboxJob", "OrderServiceJobs");
        public static readonly TriggerKey TriggerKey = new TriggerKey("PublishOutboxJob.Trigger", "OrderServiceTriggers");

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _service.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IOrderServiceContext>();
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

            var data = await dbContext.Events
                .Where(x => x.PublishStatus == EventPublishStatusEnum.Unpublished)
                .OrderBy(x => x.CreateDate)
                .Take(10)
                .ToListAsync();

            if (data.Count > 0)
            {
                Dictionary<Guid, IntegrationEvent> events = new Dictionary<Guid, IntegrationEvent>();

                foreach (var item in data)
                {
                    IntegrationEvent temp = JsonConvert.DeserializeObject(item.EventDTO, Type.GetType(item.EventType)) as IntegrationEvent;
                    events.Add(item.EventOutboxId, temp);
                }

                foreach (var @event in events)
                {
                    try
                    {
                        await eventBus.Publish(@event.Value);

                        var eventOutbox = data.FirstOrDefault(x => x.EventOutboxId == @event.Key);

                        eventOutbox.PublishStatus = EventPublishStatusEnum.Published;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Ошибка при отправке запроса: \\n{event}\\nОшибка: {error}",
                            JsonConvert.SerializeObject(@event),
                            ex.Message);

                        var ss = data.FirstOrDefault(x => x.EventOutboxId == @event.Key);

                        ss.PublishStatus = EventPublishStatusEnum.Unpublished;
                    }
                    finally
                    {
                        await dbContext.SaveChangesAsync(default);
                    }
                }
            }
        }
    }
}
