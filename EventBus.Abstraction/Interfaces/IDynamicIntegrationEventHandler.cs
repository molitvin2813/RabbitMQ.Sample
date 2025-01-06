namespace EventBus.Abstraction.Interfaces
{
    public interface IDynamicIntegrationEventHandler : IIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
