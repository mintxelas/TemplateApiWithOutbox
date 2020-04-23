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

        public Task<Message> GetById(Guid id)
            => Task.FromResult(ToMessage(dbContext.MessageRecord
                    .SingleOrDefault(m => m.Id == id)));

        public async Task Save(Message message)
        {
            var record = ToRecord(message);
            if (message.Id == default)
            {
                await dbContext.MessageRecord.AddAsync(record);
            }
            else
            {
                dbContext.MessageRecord.Update(record);
            }

            var withEvents = (IExposeEvents)message;
            foreach (var domainEvent in withEvents.PendingEvents)
            {
                var outboxEvent = new OutboxEvent
                {
                    CreatedDate = DateTimeOffset.Now,
                    EventName = domainEvent.GetType().AssemblyQualifiedName,
                    Payload = JsonSerializer.Serialize(domainEvent)
                };
                dbContext.OutboxEvent.Add(outboxEvent);
            }
            withEvents.ClearPendingEvents();

            await dbContext.SaveChangesAsync();
        }

        private Message ToMessage(MessageRecord record)
            => record is null
               ? null
               : new Message(record.Id, record.Text);

        private MessageRecord ToRecord(Message message)
            => new MessageRecord
               {
                   Id = message.Id == default ? Guid.NewGuid() : message.Id,
                   Text = message.Text
               };
    }
}
