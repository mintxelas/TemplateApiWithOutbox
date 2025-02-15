using System;

namespace Sample.Domain;

public class MatchingMessageReceived : IDomainEvent
{
    public Guid MessageId { get; set; }
}