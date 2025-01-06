using CatalogService.Application.Interfaces;
using EventBus.Abstraction.AbstractClass;
using EventBus.RabbitMQ.Interfaces;
using Newtonsoft.Json;
using Quartz;

namespace CatalogService.API.Jobs
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

        public static readonly JobKey JobKey = new JobKey("ProcessOutboxJob", "CatalogServiceJobs");
        public static readonly TriggerKey TriggerKey = new TriggerKey("ProcessOutboxJob.Trigger", "CatalogServiceTriggers");

        private readonly ILogger<ProcessOutboxJob> _logger;
        private readonly IServiceProvider _service;

        public async Task Execute(IJobExecutionContext context)
        {
            var _subsManager = _service.GetRequiredService<IEventBusSubscriptionsManager>();
            using var scope = _service.CreateScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var events = await _unitOfWork.IntegrationEventOutboxService
                .GetUnprocessedEvents(default);

            if (events.Count > 0)
            {
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
