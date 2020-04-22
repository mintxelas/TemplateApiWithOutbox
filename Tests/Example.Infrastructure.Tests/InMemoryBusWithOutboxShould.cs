using Example.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Threading;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public class InMemoryBusWithOutboxShould
    {
        private readonly ILogger<BusReaderWithOutbox> logger;
        private readonly OutboxInMemoryRepository busWriter;
        private readonly BusReaderWithOutbox busReader;

        public InMemoryBusWithOutboxShould()
        {
            logger = Substitute.For<ILogger<BusReaderWithOutbox>>();
            busWriter = new OutboxInMemoryRepository();
            busReader = new BusReaderWithOutbox(logger, busWriter);
        }

        [Fact]
        public void contain_references_to_subscribers_in_bus_reader()
        {
            var invoked = false;
            var domainEvent = new MockEvent();
            Action<MockEvent> handler = (evt) => invoked = true;
            busReader.Subscribe(handler);

            busWriter.Publish(domainEvent);
            Thread.Sleep(6 * 1000);

            Assert.True(invoked);
        }

        [Fact]
        public void invoke_subscribers_from_bus_reader_when_events_are_published_by_bus_writer()
        {
            var invokedTimes = 0;
            var domainEvent = new MockEvent();
            Action<MockEvent> handler = (evt) => invokedTimes += 1;
            busReader.Subscribe(handler);

            busWriter.Publish(domainEvent);
            busWriter.Publish(domainEvent);
            Thread.Sleep(6 * 1000);

            Assert.Equal(2, invokedTimes);
        }

        private class MockEvent:DomainEvent
        {

        }
    }
}
