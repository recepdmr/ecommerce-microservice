using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using EventBus.Tests.Events;

namespace EventBus.Tests.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        public async Task HandleAsync(OrderCreatedIntegrationEvent @event)
        {
            System.Console.WriteLine("Handler method worked with id {0}", @event.Id);

            await Task.CompletedTask;
        }
    }
}