namespace EventBus.Base
{
    public class SubscriptionInfo
    {
        private SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType;
        }

        public Type HandlerType { get; }

        public static SubscriptionInfo Create(Type handleType)
        {
            return new SubscriptionInfo(handleType);
        }
    }
}