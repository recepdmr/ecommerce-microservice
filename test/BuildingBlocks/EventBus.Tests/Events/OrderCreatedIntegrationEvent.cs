using EventBus.Base.Events;

namespace EventBus.Tests.Events
{
    public class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        public OrderCreatedIntegrationEvent()
        {

        }

        public OrderCreatedIntegrationEvent(int id)
        {
            OrderId = id;
        }

        public int OrderId { get; set; }
    }
}