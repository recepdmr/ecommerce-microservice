using EventBus.Base.Abstraction;
using EventBus.Base.Events;

namespace EventBus.Base.SubscriptionManagers
{
    public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;
        public event EventHandler<string> OnEventRemoved;
        public Func<string, string> EventNameGetter;

        public InMemoryEventBusSubscriptionManager(Func<string, string> eventNameGetter)
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
            EventNameGetter = eventNameGetter;
        }


        public bool IsEmpty => !_handlers.Keys.Any();

        public void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {

            var eventName = GetEventKey<TIntegrationEvent>();

            AddSubscription(typeof(TIntegrationEventHandler), eventName);

            if (!_eventTypes.Contains(typeof(TIntegrationEvent)))
            {
                _eventTypes.Add(typeof(TIntegrationEvent));
            }
        }

        private void AddSubscription(Type handlerType, string eventName)
        {
            if (!HasSubscriptionForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }

            if (_handlers[eventName].Any(x => x.HandlerType == handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for {eventName}", nameof(handlerType));
            }

            _handlers[eventName].Add(SubscriptionInfo.Create(handlerType));
        }


        public void Clear()
        {
            _handlers.Clear();
        }

        public Type GetEventTypeByName(string eventName) => _eventTypes.FirstOrDefault(x => x.Name == eventName);

        public IEnumerable<SubscriptionInfo> GetHandlersForEvents<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
        {
            var key = GetEventKey<TIntegrationEvent>();

            return GetHandlersForEvents(key);
        }

        public string GetEventKey<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
        {
            var eventName = typeof(TIntegrationEvent).Name;

            return EventNameGetter(eventName);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvents(string eventName) => _handlers[eventName];

        public bool HasSubscriptionForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
        {
            var eventName = GetEventKey<TIntegrationEvent>();
            return HasSubscriptionForEvent(eventName);
        }

        public bool HasSubscriptionForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var handlerToRemove = FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEventHandler>();

            var eventName = GetEventKey<TIntegrationEvent>();
            RemoveHandler(eventName, handlerToRemove);
        }

        private SubscriptionInfo FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = GetEventKey<TIntegrationEvent>();
            return FindSubscriptionToRemove(eventName, typeof(TIntegrationEventHandler));
        }

        private SubscriptionInfo FindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionForEvent(eventName))
            {
                return null;
            }

            return _handlers[eventName].SingleOrDefault(x => x.HandlerType == handlerType);
        }

        private void RemoveHandler(string eventName, SubscriptionInfo subscriptionInfo)
        {
            if (subscriptionInfo != null)
            {
                _handlers[eventName].Remove(subscriptionInfo);
            }

            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);

                var eventType = _eventTypes.FirstOrDefault(x => x.Name == eventName);

                if (eventType != null)
                {
                    _eventTypes.Remove(eventType);
                }

                RaiseOnEventRemoved(eventName);
            }
        }


        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;

            handler?.Invoke(this, eventName);
        }
    }
}