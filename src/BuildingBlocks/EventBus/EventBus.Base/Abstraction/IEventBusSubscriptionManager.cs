using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>() where TIntegrationEvent : IntegrationEvent where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
        void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>() where TIntegrationEvent : IntegrationEvent where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
        bool HasSubscriptionForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;
        bool HasSubscriptionForEvent(string eventName);

        Type GetEventTypeByName(string eventName);
        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvents<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvents(string eventName);
    }
}