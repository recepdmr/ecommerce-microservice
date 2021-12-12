using EventBus.Base;
using EventBus.Base.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Polly;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace EventBus.RabbitMQ
{
    public class RabbitMQEventBus : EventBusBase
    {
        private RabbitMQPersistanceConnection RabbitMQPersistanceConnection { get; }
        private IConnectionFactory ConnectionFactory { get; }

        private IModel ConsumerChannel { get; }

        public RabbitMQEventBus(IServiceProvider serviceProvider, EventBusConfig eventBusConfig) : base(serviceProvider, eventBusConfig)
        {
            RabbitMQPersistanceConnection = new RabbitMQPersistanceConnection(eventBusConfig.Connection as IConnectionFactory);
            ConsumerChannel = CreateConsumerChannel();

            SubscriptionManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
        }

        private void SubscriptionManager_OnEventRemoved(object? sender, string eventName)
        {
            eventName = ProgressEventName(eventName);

            if (RabbitMQPersistanceConnection.IsConnection)
            {
                RabbitMQPersistanceConnection.TryConnect();
            }

            ConsumerChannel.QueueUnbind(queue: eventName,
            exchange: EventBusConfig.DefaultTopicName,
            routingKey: eventName);

            if (SubscriptionManager.IsEmpty)
            {
                ConsumerChannel.Close();
            }
        }

        public async override Task PublishAsync(IntegrationEvent @event)
        {
            await Task.CompletedTask;
            if (!RabbitMQPersistanceConnection.IsConnection)
            {
                RabbitMQPersistanceConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {

            });

            var eventName = @event.GetType().Name;

            eventName = ProgressEventName(eventName);

            ConsumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");

            var message = JsonSerializer.Serialize(@event);

            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = ConsumerChannel.CreateBasicProperties();

                properties.DeliveryMode = 2; // persintance

                var subscriptionName = GetSubscriptionName(eventName);
                
                ConsumerChannel.QueueDeclare(queue: subscriptionName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                ConsumerChannel.QueueBind(queue: subscriptionName,
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName);


                ConsumerChannel.BasicPublish(exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName,
                mandatory: true,
                basicProperties: properties,
                body: body);
            });
        }

        public override async Task SubscibeAsync<TIntegrationEvent, TIntegrationEventHandler>()
        {
            var eventName = typeof(TIntegrationEvent).Name;

            eventName = ProgressEventName(eventName);

            if (!SubscriptionManager.HasSubscriptionForEvent(eventName))
            {
                if (RabbitMQPersistanceConnection.IsConnection)
                {
                    RabbitMQPersistanceConnection.TryConnect();
                }

                var subscriptionName = GetSubscriptionName(eventName);
                ConsumerChannel.QueueDeclare(queue: subscriptionName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

                ConsumerChannel.QueueBind(queue: subscriptionName,
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName);

                SubscriptionManager.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>();
                StartBasicConsume(eventName);
            }

            await Task.CompletedTask;
        }

        public async override Task UnSubscibeAsync<TIntegrationEvent, TIntegrationEventHandler>()
        {
            SubscriptionManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
            await Task.CompletedTask;
        }

        private IModel CreateConsumerChannel()
        {
            if (!RabbitMQPersistanceConnection.IsConnection)
            {
                RabbitMQPersistanceConnection.TryConnect();
            }

            var channel = RabbitMQPersistanceConnection.CreateModel();

            channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName,
            type: "direct");

            return channel;

        }


        private void StartBasicConsume(string eventName)
        {
            if (ConsumerChannel is not null)
            {
                var consumer = new EventingBasicConsumer(ConsumerChannel);

                consumer.Received += Consumer_Received;

                ConsumerChannel.BasicConsume(queue: GetSubscriptionName(eventName),
                autoAck: false,
                consumer: consumer);
            }
        }


        private async void Consumer_Received(object? sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            eventName = ProgressEventName(eventName);

            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEventAsync(eventName, message);
            }
            catch (System.Exception ex)
            {

                throw;
            }
            ConsumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
    }
}