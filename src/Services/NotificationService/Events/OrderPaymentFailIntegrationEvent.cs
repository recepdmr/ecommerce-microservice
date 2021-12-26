using EventBus.Base.Events;

namespace NotificationService.Events;

public class OrderPaymentFailIntegrationEvent : IntegrationEvent
{
    public OrderPaymentFailIntegrationEvent(int orderId, string errorMessage)
    {
        OrderId = orderId;
        ErrorMessage = errorMessage;
    }

    public int OrderId { get; }
    public string ErrorMessage { get; }
}