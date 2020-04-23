using System;
using System.Collections.Generic;

namespace Example.Domain
{
    public class Message: IExposeEvents
    {
        private readonly List<DomainEvent> pendingEvents = new List<DomainEvent>();
        
        public Guid Id { get; }

        public string Text { get; }

        public Message(string text)
            => (Id, Text) = (default, text);

        public Message(Guid id, string text)
            => (Id, Text) = (id, text);

        public virtual void Process(string textToMatch)
        {
            if (Text == textToMatch)
            {
                pendingEvents.Add(new MatchingMessageReceived { MessageId = Id });
            }
        }

        void IExposeEvents.ClearPendingEvents() 
            => pendingEvents.Clear();

        IEnumerable<DomainEvent> IExposeEvents.PendingEvents 
            => pendingEvents.ToArray();
    }
}
