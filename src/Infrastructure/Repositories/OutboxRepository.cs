using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Template.Domain;
using Template.Infrastructure.Entities;
using Template.Infrastructure.EntityFramework;

namespace Template.Infrastructure.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly IOutboxDbContext dbContext;

        public OutboxRepository(IOutboxDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual IEnumerable<DomainEvent> PendingEvents()
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

        private DomainEvent ToDomainEvent(OutboxEvent outboxEvent)
        {
            var eventType = Type.GetType(outboxEvent.EventName);
            var domainEvent = JsonSerializer.Deserialize(outboxEvent.Payload, eventType);
            return (DomainEvent)domainEvent;
        }
    }
}
