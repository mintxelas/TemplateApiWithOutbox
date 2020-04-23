using Example.Domain;
using Example.Infrastructure.Entities;
using Example.Infrastructure.SqLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public class BusSubscriptionsWithOutboxShould
    {
        private readonly OutboxSqLiteRepository outboxRepository;
        private readonly BusSubscriptionsWithOutbox busSubscriptions;

        public BusSubscriptionsWithOutboxShould()
        {
            var logger = Substitute.For<ILogger<BusSubscriptionsWithOutbox>>();
            outboxRepository = Substitute.For<OutboxSqLiteRepository>(new object[]{null});
            busSubscriptions = new BusSubscriptionsWithOutbox(logger, outboxRepository);
        }

        [Fact]
        public void contain_references_to_subscribers_in_bus_reader()
        {
            GivenDomainEventInOutbox();
            //var invoked = false;
            //void Handler(MockEvent evt) => invoked = true;
            var invokedTimes = 0;
            void Handler(MockEvent evt) => invokedTimes += 1;

            busSubscriptions.Subscribe((Action<MockEvent>)Handler);
            Thread.Sleep(5 * 1000);

            Assert.True(invokedTimes == 1);
        }

        [Fact]
        public void invoke_subscribers_from_bus_reader_when_events_are_published_by_bus_writer()
        {
            GivenTwoDomainEventsInOutbox();
            var invokedTimes = 0;
            void Handler(MockEvent evt) => invokedTimes += 1;

            busSubscriptions.Subscribe((Action<MockEvent>)Handler);
            Thread.Sleep(5 * 1000);

            Assert.Equal(2, invokedTimes);
        }

        private void GivenTwoDomainEventsInOutbox()
        {
            var domainEvent = new MockEvent();
            outboxRepository.PendingEvents().Returns(new[] { domainEvent, domainEvent }).AndDoes(_ =>
                outboxRepository.PendingEvents().Returns(new MockEvent[] { }));
        }

        private void GivenDomainEventInOutbox()
        {
            var domainEvent = new MockEvent();
            outboxRepository.PendingEvents().Returns(new[] {domainEvent}).AndDoes(_ => 
                outboxRepository.PendingEvents().Returns(new MockEvent[] {}));
        }

        private class MockEvent:DomainEvent { }
    }
}
