using EventBus.Base.Abstraction;
using EventBus.Base.SubscriptionManagers;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Base.Events
{
    public abstract class EventBusBase : IEventBus
    {
        public EventBusBase(IServiceProvider serviceProvider,
            EventBusConfig eventBusConfig)
        {
            ServiceProvider = serviceProvider;
            EventBusConfig = eventBusConfig;
            SubscriptionManager = new InMemoryEventBusSubscriptionManager(ProgressEventName);
        }

        private IServiceProvider ServiceProvider { get; }
        public IEventBusSubscriptionManager SubscriptionManager { get; }
        public EventBusConfig EventBusConfig { get; }

        public virtual string GetSubscriptionName(string eventName)
        {
            return $"{EventBusConfig.SubscriberClientAppName}.{ProgressEventName(eventName)}";
        }

        public virtual string ProgressEventName(string eventName)
        {
            if (EventBusConfig.DeleteEventPrefix)
            {
                eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray());
            }

            if (EventBusConfig.DeleteEventSuffix)
            {
                eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray());
            }

            return eventName;
        }


        public async Task<bool> ProcessEventAsync(string eventName, string message)
        {
            eventName = ProgressEventName(eventName);

            var processed = false;
            if (SubscriptionManager.HasSubscriptionForEvent(eventName))
            {
                var subscriptions = SubscriptionManager.GetHandlersForEvents(eventName);

                using (var scope = ServiceProvider.CreateScope())
                {
                    foreach (var subscription in subscriptions)
                    {
                        var handler = ServiceProvider.GetService(subscription.HandlerType);

                        if (handler is null) continue;

                        var eventType = SubscriptionManager.GetEventTypeByName($"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");

                        var integrationEvent = System.Text.Json.JsonSerializer.Deserialize(message, eventType);

                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        if (concreteType is not null && handler is not null && integrationEvent is not null)
                        {
                            var parameters = new object[] { integrationEvent };
                            await (Task)concreteType.GetMethod("HandleAsync").Invoke(handler, parameters);
                        }
                    }
                }
                processed = true;
            }
            return processed;
        }

        public abstract Task PublishAsync(IntegrationEvent @event);

        public abstract Task SubscibeAsync<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        public abstract Task UnSubscibeAsync<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
    }
}