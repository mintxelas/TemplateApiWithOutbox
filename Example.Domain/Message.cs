using System.Collections.Generic;

namespace Example.Domain
{
    public class Message: IExposeEvents
    {
        private readonly List<DomainEvent> pendingEvents = new List<DomainEvent>();
        
        public int Id { get; }

        public string Text { get; }
        
        public Message(int id, string text)
            => (Id, Text) = (id, text);

        public Message(string text)
            => (Id, Text) = (default, text);

        public virtual void Process(string testToMatch)
        {
            if (Text == testToMatch)
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
