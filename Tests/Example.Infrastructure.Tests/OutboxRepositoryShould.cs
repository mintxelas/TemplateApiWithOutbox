using System;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Template.Domain;
using Template.Infrastructure.Entities;
using Template.Infrastructure.SqLite;
using Xunit;

namespace Template.Infrastructure.Tests
{
    public sealed class OutboxRepositoryShould: IDisposable
    {
        private readonly OutboxSqLiteRepository repository;
        private readonly OutboxConsumerDbContext dbContext;

        public OutboxRepositoryShould()
        {
            var options = new DbContextOptionsBuilder<OutboxConsumerDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            dbContext = new OutboxConsumerDbContext(options);
            repository = new OutboxSqLiteRepository(dbContext);
        }

        [Fact]
        public void retrieve_the_stored_event()
        {
            var givenEvent = GivenEvent();
            var actualEvents = repository.PendingEvents();
            Assert.Single(actualEvents.Where(e => e.Equals(givenEvent)));
        }

        [Fact]
        public void store_multiple_events_at_once()
        {
            var givenEvent1 = GivenEvent();
            var givenEvent2 = GivenEvent();
            

            var actualEvents = repository.PendingEvents().ToArray();

            Assert.Contains(givenEvent1, actualEvents);
            Assert.Contains(givenEvent2, actualEvents);
            Assert.Equal(2, actualEvents.Count());
        }

        [Fact]
        public void not_return_an_event_after_it_has_been_already_returned()
        {
            var givenEvent1 = GivenEvent();
            _ = repository.PendingEvents().ToArray();
            var givenEvent2 = GivenEvent();

            var actualEvents = repository.PendingEvents().ToArray();

            Assert.DoesNotContain(givenEvent1, actualEvents);
            Assert.Contains(givenEvent2, actualEvents);
            Assert.Single(actualEvents);
        }
        
        private MockEvent GivenEvent()
        {
            var givenEvent = new MockEvent(){ Id = Guid.NewGuid() };
            dbContext.OutboxEvent.Add(new OutboxEvent()
            {
                CreatedDate = DateTimeOffset.Now,
                EventName = givenEvent.GetType().AssemblyQualifiedName,
                Payload = JsonSerializer.Serialize(givenEvent)
            });
            dbContext.SaveChanges();
            return givenEvent;
        }

        public void Dispose()
        {
            dbContext?.Dispose();
        }

        private class MockEvent : DomainEvent
        {
            public Guid Id { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                if (obj is MockEvent other)
                    return other.Id == Id;
                return false;
            }
        }
    }
}
