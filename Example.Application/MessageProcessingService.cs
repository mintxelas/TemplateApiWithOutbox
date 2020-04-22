using Example.Domain;

namespace Example.Application
{
    public class MessageProcessingService
    {
        private readonly IMessageRepository repository;
        private readonly IEventWriter bus;

        public MessageProcessingService(IMessageRepository repository, IEventWriter bus)
        {
            this.repository = repository;
            this.bus = bus;
        }

        public virtual void Process(int messageId, string messageToMatch)
        {
            var message = repository.GetById(messageId);
            message.Process(messageToMatch);

            repository.Save(message);

            var withEvents = (IExposeEvents)message;
            foreach (var @event in withEvents.PendingEvents)
            {
                bus.Publish(@event);
            }
            withEvents.ClearPendingEvents();
        }
    }
}
