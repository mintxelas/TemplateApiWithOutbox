using Example.Model;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Threading;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public class InMemoryBusWithOutboxShould
    {
        private readonly ILogger<InMemoryBusWithOutbox> logger;
        private readonly InMemoryBusWithOutbox bus;

        public InMemoryBusWithOutboxShould()
        {
            logger = Substitute.For<ILogger<InMemoryBusWithOutbox>>();
            bus = new InMemoryBusWithOutbox(logger, new OutboxRepository());
        }

        [Fact]
        public void contain_references_to_subscribers()
        {
            var invoked = false;
            var domainEvent = new MockEvent();
            Action<MockEvent> handler = (evt) => invoked = true;
            bus.Subscribe(handler);

            bus.Publish(domainEvent);
            Thread.Sleep(6 * 1000);

            Assert.True(invoked);
        }

        [Fact]
        public void invoke_subscribers_when_events_are_published()
        {
            var invokedTimes = 0;
            var domainEvent = new MockEvent();
            Action<MockEvent> handler = (evt) => invokedTimes += 1;
            bus.Subscribe(handler);

            bus.Publish(domainEvent);
            bus.Publish(domainEvent);
            Thread.Sleep(6 * 1000);

            Assert.Equal(2, invokedTimes);
        }

        private class MockEvent:DomainEvent
        {

        }
    }
}
