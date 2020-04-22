using Example.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Example.Infrastructure
{
    public sealed class InMemoryBus : IEventReader, IEventWriter
    {
        private readonly ConcurrentDictionary<Type, List<Action<DomainEvent>>> subscribers 
            = new ConcurrentDictionary<Type, List<Action<DomainEvent>>>();
        private readonly ILogger<InMemoryBus> logger;

        public InMemoryBus(ILogger<InMemoryBus> logger)
        {
            this.logger = logger;
        }

        public void Subscribe<T>(Action<T> handler) where T : DomainEvent
        {
            var key = typeof(T);
            var wrapper = new Action<DomainEvent>(evt => handler((T)evt));

            subscribers.AddOrUpdate(key,
                    new List<Action<DomainEvent>> { wrapper },
                    (type, handlers) =>
                    {
                        handlers.Add(wrapper);
                        return handlers;
                    });
        }

        public void Publish(DomainEvent domainEvent)
        {
            var key = domainEvent.GetType();
            if (subscribers.ContainsKey(key))
            {
                foreach (var handler in subscribers[key])
                {
                    try
                    {
                        handler(domainEvent);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Exception handling event of type {eventType}", key.Name);
                    }
                }
            }
        }

        public void Dispose()
        {
            subscribers?.Clear();
        }
    }
}
