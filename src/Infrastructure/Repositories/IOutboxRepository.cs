using System.Collections.Generic;
using Sample.Domain;

namespace Sample.Infrastructure.Repositories
{
    public interface IOutboxRepository
    {
        IEnumerable<DomainEvent> PendingEvents();
    }
}