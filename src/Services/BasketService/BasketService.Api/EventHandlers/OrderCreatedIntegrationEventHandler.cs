using BasketService.Api.Application.Repositories;
using BasketService.Api.Events;
using EventBus.Base.Abstraction;

namespace BasketService.Api.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        public OrderCreatedIntegrationEventHandler(IBasketRepository basketRepository, ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            BasketRepository = basketRepository;
            Logger = logger;
        }

        public IBasketRepository BasketRepository { get; }
        public ILogger<OrderCreatedIntegrationEventHandler> Logger { get; }

        public async Task HandleAsync(OrderCreatedIntegrationEvent @event)
        {
            Logger.LogInformation("----- Handling integration event : {IntegrationEventId} at BasketService.Api - ({@IntegrationEvent})", @event.Id);

            await BasketRepository.DeleteAsync(@event.UserId.ToString());
        }
    }
}