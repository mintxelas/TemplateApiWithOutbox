using System;

namespace Sample.Domain
{
    public interface IEventReader : IDisposable
    {
        void Subscribe<T>(Action<T> handler) where T : DomainEvent;
    }
}