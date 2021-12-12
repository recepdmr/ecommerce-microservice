using EventBus.Base.Events;

namespace PaymentService.Api.Events;
public class OrderStartedIntegrationEvent : IntegrationEvent
{
    public OrderStartedIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }

    public int OrderId { get; }
}