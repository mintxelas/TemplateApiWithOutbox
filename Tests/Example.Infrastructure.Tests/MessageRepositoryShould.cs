using System;
using Example.Domain;
using Example.Infrastructure.SqLite;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public class MessageRepositoryShould
    {
        private const string SomeText = "Some Text";
        private const string AChangedText = "changed text";
        private static readonly Guid ANotPersistedId = Guid.NewGuid();
        private static readonly Guid AnExistingMessageId = Guid.NewGuid();
        private readonly MessageSqLiteRepository repository;
        private readonly ExampleDbContext exampleDbContext;

        public MessageRepositoryShould()
        {
            var options = new DbContextOptionsBuilder<ExampleDbContext>().UseInMemoryDatabase(this.GetType().Name).Options;
            exampleDbContext = new ExampleDbContext(options);
            repository = new MessageSqLiteRepository(exampleDbContext);
        }

        [Fact]
        public async Task retrieve_a_message_by_its_id()
        {
            var expectedMessage = await GivenPersistedMessage();
            var actualMessage = await repository.GetById(expectedMessage.Id);
            Assert.Equal(expectedMessage.Id, actualMessage.Id);
        }

        [Fact]
        public void throw_key_not_found_exception_when_message_is_not_in_database()
        {
            void GetAction() => _ = repository.GetById(ANotPersistedId).Result;
            Assert.Throws<KeyNotFoundException>(GetAction);
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
        public async Task save_events_in_outbox_when_saving_changes()
        {
            var givenMessage = await GivenPersistedMessage();
            givenMessage.Process(SomeText);

            await repository.Save(givenMessage);

            var outboxEvent = exampleDbContext.OutboxEvent.Single(oe =>
                oe.EventName == typeof(MatchingMessageReceived).AssemblyQualifiedName);
            var actualEvent = (MatchingMessageReceived)JsonSerializer.Deserialize(outboxEvent.Payload, Type.GetType(outboxEvent.EventName));
            Assert.Equal(givenMessage.Id, actualEvent.MessageId);
        }

        private Message GivenUpdatedMessage()
        {
            return new Message(AnExistingMessageId, AChangedText);
        }

        private async Task<Message> GivenPersistedMessage()
        {
            var message = new Message(AnExistingMessageId, SomeText);
            await repository.Save(message);
            return message;
        }
    }
}
