using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using NotificationService.Events;

namespace NotificationService.EventHandlers
{
    public class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailIntegrationEvent>
    {

        public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
        {
            Logger = logger;
        }

        public ILogger<OrderPaymentFailedIntegrationEventHandler> Logger { get; }

        public Task HandleAsync(OrderPaymentFailIntegrationEvent @event)
        {
            Logger.LogInformation("Operation Failed {eventName} {eventHandler} {@event}", nameof(OrderPaymentFailIntegrationEvent), nameof(OrderPaymentFailedIntegrationEventHandler), @event);


            return Task.CompletedTask;
        }
    }
}