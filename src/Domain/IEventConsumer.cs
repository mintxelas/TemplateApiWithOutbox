using System;

namespace Sample.Domain;

public interface IEventConsumer : IDisposable
{
    void Subscribe<T>(Action<T> handler) where T : IDomainEvent;
}