using System;
using System.Collections.Generic;

namespace Sample.Domain;

public class Message(Guid id, string text) : IExposeEvents
{
    private readonly List<IDomainEvent> pendingEvents = new List<IDomainEvent>();
        
    public Guid Id { get; } = id;

    public string Text { get; } = text;

    public Message(string text) : this(Guid.Empty, text)
    {
    }

    public virtual void Process(string textToMatch)
    {
        if (Text == textToMatch)
        {
            pendingEvents.Add(new MatchingMessageReceived { MessageId = Id });
        }
    }

    void IExposeEvents.ClearPendingEvents() 
        => pendingEvents.Clear();

    IEnumerable<IDomainEvent> IExposeEvents.PendingEvents 
        => pendingEvents.ToArray();

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (obj is Message other) return Id == other.Id && Text == other.Text;
        return false;
    }

    protected bool Equals(Message other)
    {
        return Id.Equals(other.Id) && Text == other.Text;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Text);
    }
}