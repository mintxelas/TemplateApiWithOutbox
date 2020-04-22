using Example.Domain;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Example.Infrastructure
{
    public class OutboxRepository : IEventWriter
    {
        private readonly ConcurrentQueue<DomainEvent> queue = new ConcurrentQueue<DomainEvent>();

        public virtual void Publish(DomainEvent @event)
        {
            queue.Enqueue(@event);
        }

        public virtual IEnumerable<DomainEvent> DequeuePendingEvents()
        {
            while (queue.TryDequeue(out var @event))
            {
                yield return @event;
            }
        }

        public void Dispose()
        {
        }
    }
}