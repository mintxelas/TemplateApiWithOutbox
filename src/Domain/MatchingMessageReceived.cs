using System;

namespace Template.Domain
{
    public class MatchingMessageReceived : DomainEvent
    {
        public Guid MessageId { get; set; }
    }
}
