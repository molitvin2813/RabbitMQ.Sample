using SharedCollection.AbstractClass;

namespace SharedCollection.Events
{
    public class OrderConfirmedEvent
         : IntegrationEvent
    {
        public int ProductId { get; set; }
        public int UnitSold { get; set; }
    }
}
