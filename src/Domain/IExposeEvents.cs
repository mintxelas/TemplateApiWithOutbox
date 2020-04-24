using System.Collections.Generic;

namespace Template.Domain
{
    public interface IExposeEvents
    {
        IEnumerable<DomainEvent> PendingEvents { get; }
        void ClearPendingEvents();
    }
}
