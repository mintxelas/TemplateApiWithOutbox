using System;

namespace Example.Api.Model
{
    public interface IEventWriter : IDisposable
    {
        void Publish(DomainEvent domainEvent);
    }
}