using Example.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public class InMemoryBusShould
    {
        private readonly ILogger<InMemoryBus> logger;
        private readonly InMemoryBus bus;

        public InMemoryBusShould()
        {
            logger = Substitute.For<ILogger<InMemoryBus>>();
            bus = new InMemoryBus(logger);
        }

        [Fact]
        public void contain_references_to_subscribers()
        {
            var invoked = false;
            var domainEvent = new MockEvent();
            Action<MockEvent> handler = (evt) => invoked = true;
            bus.Subscribe(handler);

            bus.Publish(domainEvent);

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

            Assert.Equal(2, invokedTimes);
        }

        private class MockEvent:DomainEvent
        {

        }
    }
}
