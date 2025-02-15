using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Sample.Domain;
using Sample.Infrastructure.Entities;
using Sample.Infrastructure.EntityFramework;

namespace Sample.Infrastructure.Repositories;

public class OutboxRepository(IOutboxDbContext dbContext) : IOutboxRepository
{
    public virtual IEnumerable<IDomainEvent> PendingEvents()
    {
        foreach(var outboxEvent in dbContext.OutboxEvents
                    .Where(oe => !oe.ProcessedDate.HasValue)
                    .OrderBy(oe => oe.Id))
        {
            outboxEvent.ProcessedDate = DateTimeOffset.Now;
            dbContext.SaveChanges();
            yield return ToDomainEvent(outboxEvent);
        }
    }

    private IDomainEvent ToDomainEvent(OutboxEvent outboxEvent)
    {
        var eventType = Type.GetType(outboxEvent.EventName);
        var domainEvent = JsonSerializer.Deserialize(outboxEvent.Payload, eventType);
        return (IDomainEvent)domainEvent;
    }
}