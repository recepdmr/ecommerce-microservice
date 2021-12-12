using EventBus.Base.Events;

namespace EventBus.Base.Abstraction;

public interface IIntegrationEventHandler<TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
{
    Task HandleAsync(TIntegrationEvent @event);
}

public interface IIntegrationEventHandler
{

}
