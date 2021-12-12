namespace EventBus.Base
{
    public class EventBusConfig
    {
        public int ConnectionRetryCount { get; set; } = 5;
        public string DefaultTopicName => "EcommerceMicroserviceEventBus";
        public string EventBusConnectionString => string.Empty;
        public string SubscriberClientAppName { get; set; } = string.Empty;
        public string EventNamePrefix { get; set; } = string.Empty;
        public string EventNameSuffix { get; set; } = string.Empty;
        public EventBusType EventBusType { get; set; } = EventBusType.RabbitMQ;
        public object? Connection { get; set; }
        public bool DeleteEventPrefix { get; set; }
        public bool DeleteEventSuffix { get; set; }
    }

    public enum EventBusType
    {
        RabbitMQ = 0,
        AzureServiceBus = 1
    }
}