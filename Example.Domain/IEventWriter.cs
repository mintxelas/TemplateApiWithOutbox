using System;

namespace Example.Domain
{
    public interface IEventWriter : IDisposable
    {
        void Publish(DomainEvent domainEvent);
    }
}