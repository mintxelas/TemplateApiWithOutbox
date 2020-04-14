using Example.Model;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public class MessageRepositoryShould
    {
        private readonly MessageRepository repository;
        private readonly IEventWriter bus;
        private const int ANotPersistedId = 123456;
        private const string SomeText = "some text";

        public MessageRepositoryShould()
        {
            bus = Substitute.For<IEventWriter>();
            repository = new MessageRepository(bus);
        }

        [Fact]
        public void retrieve_a_message_by_its_id()
        {
            var expectedMessage = GivenPersistedMessage();
            var actualMessage = repository.GetById(expectedMessage.Id);
            Assert.Equal(expectedMessage.Id, actualMessage.Id);
        }

        [Fact]
        public void throw_key_not_found_exception_when_message_is_not_in_database()
        {
            Action getAction = () => repository.GetById(ANotPersistedId);
            Assert.Throws<KeyNotFoundException>(getAction);
        }

        [Fact]
        public void store_a_message()
        {
            var expectedMessage = GivenUpdatedMessage();

            repository.Save(expectedMessage);

            var actualMessage = repository.GetById(expectedMessage.Id);
            Assert.Equal(expectedMessage.Id, actualMessage.Id);            
        }

        [Fact]
        public void publish_events_to_bus_when_saving_changes()
        {
            var expectedMessage = GivenNotPersistedMessageWithEvents();
            repository.Save(expectedMessage);
            bus.Received().Publish(Arg.Any<DomainEvent>());
        }

        private Message GivenNotPersistedMessageWithEvents()
        {
            var msg = new Message(ANotPersistedId, SomeText);
            msg.Process(SomeText);
            return msg;
        }

        private Message GivenUpdatedMessage()
        {
            return new Message(0, "changed text");
        }

        private Message GivenPersistedMessage()
        {
            // I know this message is in the sample repo. 
            return new Message(0, "Hello");
        }
    }
}
