using System;

namespace Example.Api.Model
{
    public interface IEventReader : IDisposable
    {
        void Subscribe<T>(Action<T> handler) where T : DomainEvent;
    }
}