using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Template.Domain;
using Template.Infrastructure.Entities;

namespace Template.Infrastructure.SqLite
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
            await AddOrUpdateMessage(message);
            await AddOutboxEvents(message);
            await dbContext.SaveChangesAsync();
        }

        private async Task AddOrUpdateMessage(Message message)
        {
            if (message.Id == default)
            {
                await dbContext.MessageRecord.AddAsync(ToRecord(message));
            }
            else
            {
                var record = dbContext.MessageRecord.Single(m => m.Id == message.Id);
                record.Text = message.Text;
                dbContext.MessageRecord.Update(record);
            }
        }

        private async Task AddOutboxEvents(Message message)
        {
            var withEvents = (IExposeEvents) message;
            foreach (var domainEvent in withEvents.PendingEvents)
            {
                var outboxEvent = new OutboxEvent
                {
                    CreatedDate = DateTimeOffset.Now,
                    EventName = domainEvent.GetType().AssemblyQualifiedName,
                    Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
                };
                await dbContext.OutboxEvent.AddAsync(outboxEvent);
            }

            withEvents.ClearPendingEvents();
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
