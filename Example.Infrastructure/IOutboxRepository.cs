using Example.Domain;
using System.Collections.Generic;

namespace Example.Infrastructure
{
    public interface IOutboxRepository : IEventWriter
    {
        IEnumerable<DomainEvent> PendingEvents();
    }
}