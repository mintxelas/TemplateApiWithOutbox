using System;

namespace Example.Domain
{
    public interface IEventReader : IDisposable
    {
        void Subscribe<T>(Action<T> handler) where T : DomainEvent;
    }
}