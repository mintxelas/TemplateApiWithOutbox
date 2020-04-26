using System.Collections.Generic;
using Template.Domain;

namespace Template.Infrastructure.Repositories
{
    public interface IOutboxRepository
    {
        IEnumerable<DomainEvent> PendingEvents();
    }
}