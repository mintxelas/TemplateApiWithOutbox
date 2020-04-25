using System.Collections.Generic;
using Template.Domain;

namespace Template.Infrastructure.SqLite
{
    public interface IOutboxRepository
    {
        IEnumerable<DomainEvent> PendingEvents();
    }
}