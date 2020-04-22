namespace Example.Domain
{
    public class MatchingMessageReceived : DomainEvent
    {
        public int MessageId { get; }

        public MatchingMessageReceived(int id)
        {
            MessageId = id;
        }
    }
}
