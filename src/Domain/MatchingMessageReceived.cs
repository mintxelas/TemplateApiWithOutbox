using System;

namespace Sample.Domain
{
    public class MatchingMessageReceived : DomainEvent
    {
        public Guid MessageId { get; set; }
    }
}
