using Example.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public class OutboxRepositoryShould
    {
        private readonly OutboxRepository repository;

        public OutboxRepositoryShould()
        {
            repository = new OutboxRepository();
        }

        [Fact]
        public void retrieve_the_stored_event()
        {
            var givenEvent = new MockEvent();

            repository.Enqueue(givenEvent);

            var actualEvents = repository.DequeuePendingEvents();
            Assert.Single(actualEvents.Where(e => e == givenEvent));
        }

        [Fact]
        public void store_multiple_events_at_once()
        {
            var givenEvent1 = new MockEvent();
            var givenEvent2 = new MockEvent();
            repository.Enqueue(givenEvent1);
            repository.Enqueue(givenEvent2);

            var actualEvents = repository.DequeuePendingEvents().ToArray();

            Assert.Contains(givenEvent1, actualEvents);
            Assert.Contains(givenEvent2, actualEvents);
            Assert.Equal(2, actualEvents.Count());
        }

        [Fact]
        public void not_return_an_event_after_it_has_been_already_returned()
        {
            var givenEvent1 = new MockEvent();
            var givenEvent2 = new MockEvent();
            repository.Enqueue(givenEvent1);
            _ = repository.DequeuePendingEvents().ToArray();
            repository.Enqueue(givenEvent2);

            var actualEvents = repository.DequeuePendingEvents().ToArray();

            Assert.DoesNotContain(givenEvent1, actualEvents);
            Assert.Contains(givenEvent2, actualEvents);
            Assert.Single(actualEvents);
        }

        private class MockEvent:DomainEvent { }
    }
}
