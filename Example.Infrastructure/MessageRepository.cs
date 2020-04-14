using Example.Model;

namespace Example.Infrastructure
{
    public class MessageRepository : IMessageRepository
    {
        private readonly Message[] database = new[] { 
            new Message(0, "Hello"), 
            new Message(1, "One"), 
            new Message(2, "Two"), 
            new Message(3, "Three") 
        };
        private readonly IEventWriter bus;

        public MessageRepository(IEventWriter bus)
        {
            this.bus = bus;
        }

        public Message GetById(int id)
        {
            return database[id];
        }

        public void Save(Message message)
        {
            database[message.Id] = message;

            var withEvents = (IExposeEvents)message;
            foreach(var @event in withEvents.PendingEvents)
            {
                bus.Publish(@event);
            }
            withEvents.ClearPendingEvents();
        }
    }
}
