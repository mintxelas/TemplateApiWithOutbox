using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Example.Domain;
using Example.Infrastructure.Entities;

namespace Example.Infrastructure.SqLite
{
    public class MessageSqLiteRepository : IMessageRepository
    {
        private readonly ExampleDbContext dbContext;

        public MessageSqLiteRepository(ExampleDbContext dbContext) 
            => this.dbContext = dbContext;

        public Task<Message> GetById(int id)
            => Task.FromResult(ToMessage(dbContext.MessageRecord
                    .SingleOrDefault(m => m.Id == id)));

        public Task Save(Message message)
        {
            dbContext.Add(ToRecord(message));
            var withEvents = (IExposeEvents)message;
            foreach (var domainEvent in withEvents.PendingEvents)
            {
                var outboxEvent = new OutboxEvent
                {
                    CreatedDate = DateTimeOffset.Now,
                    EventName = domainEvent.GetType().AssemblyQualifiedName,
                    Payload = JsonSerializer.Serialize(domainEvent)
                };
                dbContext.Add(outboxEvent);
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
