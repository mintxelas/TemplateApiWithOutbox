using System.Collections.Generic;

namespace Example.Domain
{
    public interface IExposeEvents
    {
        IEnumerable<DomainEvent> PendingEvents { get; }
        void ClearPendingEvents();
    }
}
