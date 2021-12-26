using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using NotificationService.Events;

namespace NotificationService.EventHandlers
{
    public class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
    {

        public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEvent> logger)
        {
            Logger = logger;
        }

        public ILogger<OrderPaymentSuccessIntegrationEvent> Logger { get; }

        public Task HandleAsync(OrderPaymentSuccessIntegrationEvent @event)
        {
            Logger.LogInformation(" Operation Success {@eventName} {@eventHandler} {@event}", nameof(OrderPaymentFailIntegrationEvent), nameof(OrderPaymentFailedIntegrationEventHandler), @event);

            return Task.CompletedTask;
        }
    }
}