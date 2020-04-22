using Example.Model;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Example.Infrastructure
{
    public class OutboxRepository
    {
        private readonly ConcurrentQueue<DomainEvent> queue = new ConcurrentQueue<DomainEvent>();

        public virtual void Enqueue(DomainEvent @event)
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


    }
}