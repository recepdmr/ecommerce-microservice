using EventBus.Base.Events;
namespace EventBus.Base.Abstraction
{
    public interface IEventBus
    {
        Task PublishAsync(IntegrationEvent @event);
        Task SubscibeAsync<TIntegrationEvent, TIntegrationEventHandler>() where TIntegrationEvent : IntegrationEvent where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
        Task UnSubscibeAsync<TIntegrationEvent, TIntegrationEventHandler>() where TIntegrationEvent : IntegrationEvent where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
    }
}