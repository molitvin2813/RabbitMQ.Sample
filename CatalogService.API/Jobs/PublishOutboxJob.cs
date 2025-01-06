using CatalogService.Application.Interfaces;
using EventBus.Abstraction.Interfaces;
using Newtonsoft.Json;
using Quartz;
using SharedCollection.Enums;

namespace CatalogService.API.Jobs
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

        public static readonly JobKey JobKey = new JobKey("PublishOutboxJob", "CatalogServiceJobs");
        public static readonly TriggerKey TriggerKey = new TriggerKey("PublishOutboxJob.Trigger", "CatalogServiceTriggers");

        private readonly ILogger<PublishOutboxJob> _logger;
        private readonly IServiceProvider _service;

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _service.CreateScope();
            var _eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var events = await _unitOfWork.IntegrationEventOutboxService
                .GetUnsentEvents(10, default);

            if (events.Count > 0)
            {
                foreach (var item in events)
                {
                    try
                    {
                        await _eventBus.Publish(item.Value);
                        await _unitOfWork.IntegrationEventOutboxService.ChangeEventPublishStatusAsync(
                            item.Key,
                            null,
                            EventPublishStatusEnum.Published,
                            default);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Ошибка при отправке запроса: \\n{event}\\nОшибка: {error}",
                            JsonConvert.SerializeObject(item),
                            ex.Message);

                        await _unitOfWork.IntegrationEventOutboxService.ChangeEventPublishStatusAsync(
                           item.Key,
                           null,
                           EventPublishStatusEnum.PublishFailed,
                           default);
                    }
                    finally
                    {
                        await _unitOfWork.SaveChangesAsync(default);
                    }
                }
            }
        }
    }
}