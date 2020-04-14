using System.Collections.Generic;

namespace Example.Model
{
    public interface IExposeEvents
    {
        IEnumerable<DomainEvent> PendingEvents { get; }
        void ClearPendingEvents();
    }
}
