using System.Threading;
using System.Threading.Tasks;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.RabbitMQ;
using EventBus.Tests.EventHandlers;
using EventBus.Tests.Events;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Xunit;

namespace EventBus.Tests
{
    public class RabbitMQEventBusTests
    {

        [Fact]
        public async Task Subscribe_Event_On_RabbitMQAsync()
        {
            // Given
            IEventBus eventBus = GetEventBus();

            // When

            await eventBus.PublishAsync(new OrderCreatedIntegrationEvent(122));

            await eventBus.SubscibeAsync<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            Task.Delay(22000).Wait();
            // Then
        }

        [Fact]

        public async Task Consume_OrderCreated_From_RabbitMQ()
        {
            // Given
            IEventBus eventBus = GetEventBus();

            // When

            await eventBus.SubscibeAsync<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            // Then
            Task.Delay(22000).Wait();
        }

        private static IEventBus GetEventBus()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddSingleton<OrderCreatedIntegrationEventHandler>();
            services.AddSingleton<IEventBus>(sp =>
            {
                var eventBusConfig = new EventBusConfig
                {
                    ConnectionRetryCount = 5,
                    SubscriberClientAppName = "EventBus.Tests",
                    EventBusType = EventBusType.RabbitMQ,
                    EventNameSuffix = "IntegrationEvent",
                    Connection = new ConnectionFactory(),
                    DeleteEventSuffix = true,
                };
                return EventBusFactory.Create(eventBusConfig, sp);
            });

            var serviceProvider = services.BuildServiceProvider();

            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            return eventBus;
        }
    }
}