using System.Collections.Generic;

namespace Example.Api.Model
{
    public interface IExposeEvents
    {
        IEnumerable<DomainEvent> PendingEvents { get; }
        void ClearPendingEvents();
    }
}
