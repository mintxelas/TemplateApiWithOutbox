using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sample.Domain;
using Sample.Infrastructure.Entities;
using Sample.Infrastructure.EntityFramework;
using Sample.Infrastructure.Repositories;
using Xunit;

namespace Sample.Infrastructure.Tests
{
    public sealed class MessageRepositoryShould : IDisposable
    {
        private const string SomeText = "Some Text";
        private const string AChangedText = "changed text";
        private static readonly Guid APersistedId = Guid.NewGuid();
        private readonly MessageRepository repository;
        private readonly MessageDbContext messageDbContext;

        public MessageRepositoryShould()
        {
            var options = new DbContextOptionsBuilder<MessageDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            messageDbContext = new MessageDbContext(options);
            repository = new MessageRepository(messageDbContext);
        }

        [Fact]
        public async Task retrieve_all_messages()
        {
            var expectedMessage = await GivenPersistedMessage();
            var actualMessages = await repository.GetAll();
            Assert.Contains(expectedMessage, actualMessages);
        }

        [Fact]
        public async Task retrieve_a_message_by_its_id()
        {
            var expectedMessage = await GivenPersistedMessage();
            var actualMessage = await repository.GetById(expectedMessage.Id);
            Assert.Equal(expectedMessage.Id, actualMessage.Id);
        }

        [Fact]
        public async Task store_a_message()
        {
            var expectedMessage = GivenNewMessage();

            await repository.Save(expectedMessage);

            var actualMessage = messageDbContext.MessageRecords.Single();
            Assert.Equal(expectedMessage.Text, actualMessage.Text);
        }

        [Fact]
        public async Task save_events_in_outbox_when_saving_changes()
        {
            var givenMessage = await GivenPersistedMessage();
            givenMessage.Process(SomeText);

            var actualMessage = await repository.Save(givenMessage);

            var outboxEvent = messageDbContext.OutboxEvents.Single(oe =>
                oe.EventName == typeof(MatchingMessageReceived).AssemblyQualifiedName);
            var actualEvent = (MatchingMessageReceived)JsonSerializer.Deserialize(outboxEvent.Payload, Type.GetType(outboxEvent.EventName));
            Assert.Equal(givenMessage.Id, actualEvent.MessageId);
        }

        private Message GivenNewMessage()
        {
            return new Message(default, AChangedText);
        }

        private async Task<Message> GivenPersistedMessage()
        {
            var messageRecord = new MessageRecord()
            {
                Id = APersistedId,
                Text = SomeText
            };
            _ = await messageDbContext.MessageRecords.AddAsync(messageRecord);
            await messageDbContext.SaveChangesAsync();
            return new Message(APersistedId, SomeText);
        }

        public void Dispose()
        {
            messageDbContext?.Dispose();
        }
    }
}
