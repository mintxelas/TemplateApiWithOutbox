using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sample.Domain;
using Sample.Infrastructure.Repositories;

namespace Sample.Infrastructure.Subscriptions
{
    public sealed class BusSubscriptionsWithOutbox : IEventReader
    {
        private static readonly ConcurrentDictionary<Type, List<Action<DomainEvent>>> Subscribers 
            = new ConcurrentDictionary<Type, List<Action<DomainEvent>>>();

        private readonly ILogger<BusSubscriptionsWithOutbox> logger;
        private readonly IOutboxRepository repository;
        private readonly RepeatingTimer timer;

        public BusSubscriptionsWithOutbox(ILogger<BusSubscriptionsWithOutbox> logger, IOutboxRepository repository, RepeatingTimer timer)
        {
            this.logger = logger;
            this.repository = repository;
            this.timer = timer;
            this.timer.OnTick = OnTick;
        }

        public void Subscribe<T>(Action<T> handler) where T : DomainEvent
        {
            var key = typeof(T);
            var wrapper = new Action<DomainEvent>(evt => handler((T)evt));

            Subscribers.AddOrUpdate(key,
                    new List<Action<DomainEvent>> { wrapper },
                    (type, handlers) =>
                    {
                        handlers.Add(wrapper);
                        return handlers;
                    });
        }

        private void OnTick()
        {
            foreach (var @event in repository.PendingEvents())
            {
                Publish(@event);
            }
        }

        private void Publish(DomainEvent domainEvent)
        {
            var key = domainEvent.GetType();
            if (Subscribers.ContainsKey(key))
            {
                foreach (var handler in Subscribers[key])
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
            Subscribers?.Clear();
            timer?.Dispose();
        }
    }
}
