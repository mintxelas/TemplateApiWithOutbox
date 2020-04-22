using Example.Domain;
using Example.Infrastructure.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Infrastructure
{
    public class MessageSqLiteRepository : IMessageRepository
    {
        private readonly IEventWriter bus;
        private readonly ExampleDbContext dbContext;

        public MessageSqLiteRepository(IEventWriter bus, ExampleDbContext dbContext) 
            => (this.bus, this.dbContext) = (bus, dbContext);

        public Task<Message> GetById(int id)
            => Task.FromResult(ToMessage(dbContext.Messages
                    .SingleOrDefault(m => m.Id == id)));

        public Task Save(Message message)
        {
            dbContext.Add(ToRecord(message));
            var withEvents = (IExposeEvents)message;
            foreach (var @event in withEvents.PendingEvents)
            {
                bus.Publish(@event);
            }
            withEvents.ClearPendingEvents();
            return dbContext.SaveChangesAsync();
        }

        private Message ToMessage(MessageRecord record)
            => record is null
               ? null
               : new Message(record.Id, record.Text);

        private MessageRecord ToRecord(Message message)
            => new MessageRecord
               {
                   Id = message.Id,
                   Text = message.Text
               };
    }
}
