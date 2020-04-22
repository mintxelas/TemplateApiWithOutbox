using Example.Domain;
using System.Linq;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public class OutboxRepositoryShould
    {
        private readonly OutboxInMemoryRepository repository;

        public OutboxRepositoryShould()
        {
            repository = new OutboxInMemoryRepository();
        }

        [Fact]
        public void retrieve_the_stored_event()
        {
            var givenEvent = new MockEvent();

            repository.Publish(givenEvent);

            var actualEvents = repository.PendingEvents();
            Assert.Single(actualEvents.Where(e => e == givenEvent));
        }

        [Fact]
        public void store_multiple_events_at_once()
        {
            var givenEvent1 = new MockEvent();
            var givenEvent2 = new MockEvent();
            repository.Publish(givenEvent1);
            repository.Publish(givenEvent2);

            var actualEvents = repository.PendingEvents().ToArray();

            Assert.Contains(givenEvent1, actualEvents);
            Assert.Contains(givenEvent2, actualEvents);
            Assert.Equal(2, actualEvents.Count());
        }

        [Fact]
        public void not_return_an_event_after_it_has_been_already_returned()
        {
            var givenEvent1 = new MockEvent();
            var givenEvent2 = new MockEvent();
            repository.Publish(givenEvent1);
            _ = repository.PendingEvents().ToArray();
            repository.Publish(givenEvent2);

            var actualEvents = repository.PendingEvents().ToArray();

            Assert.DoesNotContain(givenEvent1, actualEvents);
            Assert.Contains(givenEvent2, actualEvents);
            Assert.Single(actualEvents);
        }

        private class MockEvent:DomainEvent { }
    }
}
