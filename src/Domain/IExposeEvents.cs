using System.Collections.Generic;

namespace Sample.Domain;

public interface IExposeEvents
{
    IEnumerable<IDomainEvent> PendingEvents { get; }
    void ClearPendingEvents();
}