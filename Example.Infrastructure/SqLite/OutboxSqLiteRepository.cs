using Example.Domain;
using Example.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Example.Infrastructure.SqLite
{
    public class OutboxSqLiteRepository 
    {
        private readonly OutboxConsumerDbContext dbContext;

        public OutboxSqLiteRepository(OutboxConsumerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<DomainEvent> PendingEvents()
        {
            foreach(var outboxEvent in dbContext.OutboxEvent.Where(oe => !oe.ProcessedDate.HasValue))
            {
                outboxEvent.ProcessedDate = DateTimeOffset.Now;
                yield return ToDomainEvent(outboxEvent);
            }
            dbContext.SaveChanges();
        }

        private DomainEvent ToDomainEvent(OutboxEvent outboxEvent)
        {
            var eventType = Type.GetType(outboxEvent.EventName);
            var domainEvent = JsonSerializer.Deserialize(outboxEvent.Payload, eventType);
            return (DomainEvent)domainEvent;
        }
    }
}
