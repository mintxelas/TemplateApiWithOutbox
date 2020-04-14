using System.Collections.Generic;

namespace Example.Model
{
    public class Message: IExposeEvents
    {
        private readonly List<DomainEvent> pendingEvents = new List<DomainEvent>();
        private readonly string message;

        public int Id { get; }
        
        public Message(int id, string message)
            => (Id, this.message) = (id, message);

        public void Process(string testToMatch)
        {
            if (message == testToMatch)
            {
                pendingEvents.Add(new MatchingMessageReceived(Id));
            }
        }

        void IExposeEvents.ClearPendingEvents()
        {
            pendingEvents.Clear();
        }

        IEnumerable<DomainEvent> IExposeEvents.PendingEvents 
            => pendingEvents.ToArray();
    }
}
