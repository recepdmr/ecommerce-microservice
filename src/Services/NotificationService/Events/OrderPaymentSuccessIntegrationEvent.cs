using EventBus.Base.Events;

namespace NotificationService.Events;

public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
{
    public OrderPaymentSuccessIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }

    public int OrderId { get; }
}