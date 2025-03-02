using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Sample.Domain;
using Sample.Infrastructure.Entities;
using Sample.Infrastructure.EntityFramework;

namespace Sample.Infrastructure.Repositories;

public class MessageRepository(MessageDbContext dbContext) : IMessageRepository
{
    public Task<Message[]> GetAll()
        => Task.FromResult(dbContext.MessageRecords.Select(MessageExtensions.ToMessage).ToArray());

    public Task<Message> GetById(Guid id)
        => Task.FromResult(dbContext.MessageRecords
            .SingleOrDefault(m => m.Id == id).ToMessage());

    public async Task<Message> Save(Message message)
    {
        var savedMessage = await AddOrUpdateMessage(message);
        await AddOutboxEvents(message);
        await dbContext.SaveChangesAsync();
        return savedMessage;
    }

    private async Task<Message> AddOrUpdateMessage(Message message)
    {
        EntityEntry<MessageRecord> entityEntry;
        if (message.Id == Guid.Empty)
        {
            entityEntry = await dbContext.MessageRecords.AddAsync(message.ToRecord());
        }
        else
        {
            var record = dbContext.MessageRecords.Single(m => m.Id == message.Id);
            record.Text = message.Text;
            entityEntry = dbContext.MessageRecords.Update(record);
        }

        return entityEntry.Entity.ToMessage();
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

    
}

internal static class MessageExtensions
{
    internal static Message ToMessage(this MessageRecord record)
        => record is null
            ? null
            : new Message(record.Id, record.Text);

    internal static MessageRecord ToRecord(this Message message)
        => new MessageRecord
        {
            Id = message.Id == Guid.Empty ? Guid.NewGuid() : message.Id,
            Text = message.Text
        };
}