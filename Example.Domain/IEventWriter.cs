using System;

namespace Example.Domain
{
    public interface IEventWriter 
    {
        void Publish(DomainEvent domainEvent);
    }
}