using SharedCollection.Interfaces;

namespace SharedCollection.AbstractClass
{
    public abstract class IntegrationEvent
        : IIntegrationEvent
    {
        public Guid IntegrationEventId { get; set; }
    }
}
