using Example.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Example.Infrastructure
{
    public sealed class BusReaderWithOutbox : IEventReader
    {
        private readonly ConcurrentDictionary<Type, List<Action<DomainEvent>>> subscribers 
            = new ConcurrentDictionary<Type, List<Action<DomainEvent>>>();
        private readonly System.Threading.Timer timer;
        private readonly ILogger<BusReaderWithOutbox> logger;
        private readonly IOutboxRepository repository;

        public BusReaderWithOutbox(ILogger<BusReaderWithOutbox> logger, IOutboxRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
            timer = new System.Threading.Timer(OnTick, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5));
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

        private void OnTick(object state)
        {
            foreach (var @event in repository.PendingEvents())
            {
                Send(@event);
            }
        }

        private void Send(DomainEvent domainEvent)
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
            timer?.Dispose();
        }
    }
}
