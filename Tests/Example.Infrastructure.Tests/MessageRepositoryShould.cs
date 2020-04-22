using Example.Domain;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public class MessageRepositoryShould
    {
        private const int ANotPersistedId = 123456;
        private const string SomeText = "Some Text";
        private const int AnExistingMessageId = 0;
        private const string AChangedText = "changed text";
        private readonly MessageRepository repository;
        private readonly IEventWriter bus;

        public MessageRepositoryShould()
        {
            bus = Substitute.For<IEventWriter>();
            repository = new MessageRepository(bus);
        }

        [Fact]
        public async Task retrieve_a_message_by_its_id()
        {
            var expectedMessage = GivenPersistedMessage();
            var actualMessage = await repository.GetById(expectedMessage.Id);
            Assert.Equal(expectedMessage.Id, actualMessage.Id);
        }

        [Fact]
        public void throw_key_not_found_exception_when_message_is_not_in_database()
        {
            Action getAction = () => _ = repository.GetById(ANotPersistedId).Result;
            Assert.Throws<KeyNotFoundException>(getAction);
        }

        [Fact]
        public async Task store_a_message()
        {
            var expectedMessage = GivenUpdatedMessage();

            await repository.Save(expectedMessage);

            var actualMessage = await repository.GetById(expectedMessage.Id);
            Assert.Equal(expectedMessage.Id, actualMessage.Id);
        }

        [Fact]
        public async Task publish_events_to_bus_when_saving_changes()
        {
            var givenMessage = GivenPersistedMessage();
            givenMessage.Process(SomeText);

            await repository.Save(givenMessage);
            
            bus.Received().Publish(Arg.Is<MatchingMessageReceived>(e =>
                e.MessageId == AnExistingMessageId));
        }

        private Message GivenUpdatedMessage()
        {
            return new Message(AnExistingMessageId, AChangedText);
        }

        private Message GivenPersistedMessage()
        {
            // I know this message is in the sample repo. 
            return new Message(AnExistingMessageId, SomeText);
        }
    }
}
