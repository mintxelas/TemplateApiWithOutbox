using System;

namespace Example.Domain
{
    public class MatchingMessageReceived : DomainEvent
    {
        public Guid MessageId { get; set; }
    }
}
