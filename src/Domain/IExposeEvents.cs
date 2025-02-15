using System.Collections.Generic;

namespace Sample.Domain
{
    public interface IExposeEvents
    {
        IEnumerable<DomainEvent> PendingEvents { get; }
        void ClearPendingEvents();
    }
}
