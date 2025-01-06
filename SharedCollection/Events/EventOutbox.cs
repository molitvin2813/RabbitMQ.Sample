using SharedCollection.Enums;

namespace SharedCollection.Events
{
    public class EventOutbox
    {
        public Guid EventOutboxId { get; set; }

        public string EventDTO { get; set; } = null!;

        public string EventType { get; set; } = null!;

        public EventPublishStatusEnum? PublishStatus { get; set; }

        public EventProcessingStatusEnum? ProcessingStatus { get; set; }

        public bool IsIncomeEvent { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
