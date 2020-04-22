using Example.Domain;
using Example.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Example.Infrastructure
{
    public class OutboxSqLiteRepository : IOutboxRepository
    {
        private readonly ExampleDbContext dbContext;

        public OutboxSqLiteRepository(ExampleDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<DomainEvent> PendingEvents()
        {
            foreach(var outboxEvent in dbContext.Outbox.Where(oe => !oe.ProcessedDate.HasValue))
            {
                outboxEvent.ProcessedDate = DateTime.Now;
                yield return ToDomainEvent(outboxEvent);
            }
            dbContext.SaveChanges();
        }

        public void Publish(DomainEvent domainEvent)
        {
            var outboxEvent = new OutboxEvent
            {
                CreatedDate = DateTime.Now,
                EventName = domainEvent.GetType().AssemblyQualifiedName,
                Payload = JsonSerializer.Serialize(domainEvent)
            };
            dbContext.Add(outboxEvent);
        }

        private DomainEvent ToDomainEvent(OutboxEvent outboxEvent)
        {
            var eventType = Type.GetType(outboxEvent.EventName);
            var domainEvent = JsonSerializer.Deserialize(outboxEvent.Payload, eventType);
            return (DomainEvent)domainEvent;
        }
    }
}
