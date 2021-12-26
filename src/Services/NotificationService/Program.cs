using EventBus.Base;
using NotificationService.EventHandlers;
using NotificationService.Events;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddLogging((logging) => { logging.AddConsole(); });
services.AddSingleton<IEventBus>((sp) =>
{
    var eventBusConfig = new EventBusConfig
    {
        ConnectionRetryCount = 5,
        SubscriberClientAppName = "NotificationService",
        EventBusType = EventBusType.RabbitMQ,
        EventNameSuffix = "IntegrationEvent",
        Connection = new ConnectionFactory(),
        DeleteEventSuffix = true,
    };
    return EventBusFactory.Create(eventBusConfig, sp);
});

services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();

var serviceProvider = services.BuildServiceProvider();


var eventBus = serviceProvider.GetRequiredService<IEventBus>();

eventBus.SubscibeAsync<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();
eventBus.SubscibeAsync<OrderPaymentFailIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();

Console.WriteLine("Application is Running ....");

Console.ReadLine();
