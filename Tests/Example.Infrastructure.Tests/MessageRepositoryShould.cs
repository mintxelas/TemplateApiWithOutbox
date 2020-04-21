using Example.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public class MessageRepositoryShould
    {
        private readonly MessageRepository repository;
        private const int ANotPersistedId = 123456;

        public MessageRepositoryShould()
        {
            repository = new MessageRepository();
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
