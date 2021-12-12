namespace EventBus.Base.Events
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
        }
        
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}