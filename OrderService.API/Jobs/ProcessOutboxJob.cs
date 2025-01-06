using EventBus.Abstraction.AbstractClass;
using EventBus.RabbitMQ.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderService.Application.Interfaces;
using Quartz;
using SharedCollection.AbstractClass;
using SharedCollection.Enums;

namespace OrderService.API.Jobs
{
    public class ProcessOutboxJob : IJob
    {
        public ProcessOutboxJob(
            ILogger<ProcessOutboxJob> logger,
            IServiceProvider service)
        {
            _logger = logger;
            _service = service;
        }

        public static readonly JobKey JobKey = new JobKey("ProcessOutboxJob", "OrderServiceJobs");
        public static readonly TriggerKey TriggerKey = new TriggerKey("ProcessOutboxJob.Trigger", "OrderServiceTriggers");

        private readonly ILogger<ProcessOutboxJob> _logger;
        private readonly IServiceProvider _service;

        public async Task Execute(IJobExecutionContext context)
        {
            var _subsManager = _service.GetRequiredService<IEventBusSubscriptionsManager>();
            using var scope = _service.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IOrderServiceContext>();

            var data = await dbContext.Events
                .Where(x =>
                    x.ProcessingStatus == EventProcessingStatusEnum.Income ||
                    x.ProcessingStatus == EventProcessingStatusEnum.InProgress
                )
                .OrderBy(x => x.CreateDate)
                .Take(10)
                .Select(x => new
                {
                    x.EventDTO,
                    x.EventType,
                    x.EventOutboxId
                })
                .ToListAsync();

            if (data.Count > 0)
            {
                Dictionary<Guid, IntegrationEvent> events = new Dictionary<Guid, IntegrationEvent>();

                foreach (var item in data)
                {
                    IntegrationEvent temp = JsonConvert.DeserializeObject(item.EventDTO, Type.GetType(item.EventType)) as IntegrationEvent;
                    events.Add(item.EventOutboxId, temp);
                }

                foreach (var item in events)
                {
                    try
                    {
                        _logger.LogTrace("Обработка события: {EventName}", item.Value.GetType().Name);

                        if (_subsManager.HasSubscriptionsForEvent(item.Value.GetType().Name))
                        {
                            var handlers = _subsManager.GetHandlersForEvent(item.Value);

                            foreach (var handler in handlers)
                            {
                                var concreteType = typeof(Wrapper<>).MakeGenericType(item.Value.GetType());
                                var wrapper = Activator.CreateInstance(concreteType) ??
                                    throw new InvalidOperationException($"Could not create wrapper type for {concreteType}");
                                var wrapperBase = (WrapperBase)wrapper;

                                await wrapperBase.Handle(item.Value, _service, default);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Нет обработчика для события: {EventName}", item.Value.GetType().Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Ошибка при отправке запроса: \\n{event}\\nОшибка: {error}",
                            JsonConvert.SerializeObject(item),
                            ex.Message);
                    }
                }
            }
        }
    }
}
