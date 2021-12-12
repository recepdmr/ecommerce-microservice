using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using PaymentService.Api.Events;

namespace PaymentService.Api.EventHandlers
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        public OrderStartedIntegrationEventHandler(IConfiguration configuration,
        IEventBus eventBus,
        ILogger<OrderStartedIntegrationEventHandler> logger)
        {
            Configuration = configuration;
            EventBus = eventBus;
            Logger = logger;
        }

        public IConfiguration Configuration { get; }
        public IEventBus EventBus { get; }
        public ILogger<OrderStartedIntegrationEventHandler> Logger { get; }

        public async Task HandleAsync(OrderStartedIntegrationEvent @event)
        {
            var paymentResultFlag = Configuration.GetSection("PaymentResult").Get<bool>();

            IntegrationEvent newEvent = paymentResultFlag ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId) : new OrderPaymentFailIntegrationEvent(@event.OrderId, "This is fake payment api");

            Logger.LogInformation("{EventHandler}, {Event} Published : {PaymentResultFlag}",
                nameof(OrderStartedIntegrationEventHandler),
                nameof(OrderStartedIntegrationEvent),
                paymentResultFlag
             );

            await EventBus.PublishAsync(newEvent);
        }
    }
}