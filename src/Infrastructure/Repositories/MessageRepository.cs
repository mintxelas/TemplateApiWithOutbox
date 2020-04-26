using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Template.Domain;
using Template.Infrastructure.Entities;
using Template.Infrastructure.EntityFramework;

namespace Template.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ExampleDbContext dbContext;

        public MessageRepository(ExampleDbContext dbContext)
            => this.dbContext = dbContext;

        public Task<Message> GetById(Guid id)
            => Task.FromResult(ToMessage(dbContext.MessageRecords
                    .SingleOrDefault(m => m.Id == id)));

        public async Task<Message> Save(Message message)
        {
            var savedMessage = await AddOrUpdateMessage(message);
            await AddOutboxEvents(message);
            await dbContext.SaveChangesAsync();
            return savedMessage;
        }

        private async Task<Message> AddOrUpdateMessage(Message message)
        {
            EntityEntry<MessageRecord> savedEntity;
            if (message.Id == default)
            {
                savedEntity = await dbContext.MessageRecords.AddAsync(ToRecord(message));
            }
            else
            {
                var record = dbContext.MessageRecords.Single(m => m.Id == message.Id);
                record.Text = message.Text;
                savedEntity = dbContext.MessageRecords.Update(record);
            }

            return ToMessage(savedEntity.Entity);
        }

        private async Task AddOutboxEvents(Message message)
        {
            var withEvents = (IExposeEvents)message;
            foreach (var domainEvent in withEvents.PendingEvents)
            {
                var outboxEvent = new OutboxEvent
                {
                    CreatedDate = DateTimeOffset.Now,
                    EventName = domainEvent.GetType().AssemblyQualifiedName,
                    Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
                };
                await dbContext.OutboxEvents.AddAsync(outboxEvent);
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
