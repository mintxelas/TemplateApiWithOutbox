using System;

namespace Example.Model
{
    public interface IEventWriter : IDisposable
    {
        void Publish(DomainEvent domainEvent);
    }
}