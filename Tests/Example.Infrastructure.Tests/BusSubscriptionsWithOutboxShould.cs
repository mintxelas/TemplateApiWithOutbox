using Example.Domain;
using Example.Infrastructure.SqLite;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Threading;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public class BusSubscriptionsWithOutboxShould
    {
        private readonly BusSubscriptionsWithOutbox busReader;

        public BusSubscriptionsWithOutboxShould()
        {
            var logger = Substitute.For<ILogger<BusSubscriptionsWithOutbox>>();
            var busWriter = new OutboxSqLiteRepository(new OutboxConsumerDbContext());
            busReader = new BusSubscriptionsWithOutbox(logger, busWriter);
        }

        [Fact]
        public void contain_references_to_subscribers_in_bus_reader()
        {
            var invoked = false;
            var domainEvent = new MockEvent();
            void Handler(MockEvent evt) => invoked = true;
            busReader.Subscribe((Action<MockEvent>) Handler);

            Thread.Sleep(6 * 1000);

            Assert.True(invoked);
        }

        [Fact]
        public void invoke_subscribers_from_bus_reader_when_events_are_published_by_bus_writer()
        {
            var invokedTimes = 0;
            var domainEvent = new MockEvent();
            void Handler(MockEvent evt) => invokedTimes += 1;
            busReader.Subscribe((Action<MockEvent>)Handler);

            Thread.Sleep(6 * 1000);

            Assert.Equal(2, invokedTimes);
        }

        private class MockEvent:DomainEvent
        {

        }
    }
}
