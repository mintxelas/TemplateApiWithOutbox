namespace Example.Domain
{
    public class MatchingMessageReceived : DomainEvent
    {
        public int MessageId { get; set; }
    }
}
