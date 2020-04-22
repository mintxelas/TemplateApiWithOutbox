using System.Collections.Generic;

namespace Example.Model
{
    public class Message: IExposeEvents
    {
        private readonly List<DomainEvent> pendingEvents = new List<DomainEvent>();
        
        public int Id { get; }

        public string Text { get; }
        
        public Message(int id, string message)
            => (Id, Text) = (id, message);

        public virtual void Process(string testToMatch)
        {
            if (Text == testToMatch)
            {
                pendingEvents.Add(new MatchingMessageReceived(Id));
            }
        }

        void IExposeEvents.ClearPendingEvents() 
            => pendingEvents.Clear();

        IEnumerable<DomainEvent> IExposeEvents.PendingEvents 
            => pendingEvents.ToArray();
    }
}
