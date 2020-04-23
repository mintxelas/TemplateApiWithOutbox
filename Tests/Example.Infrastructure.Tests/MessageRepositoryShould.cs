using Example.Domain;
using Example.Infrastructure.Entities;
using Example.Infrastructure.SqLite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Example.Infrastructure.Tests
{
    public sealed class MessageRepositoryShould : IDisposable
    {
        private const string SomeText = "Some Text";
        private const string AChangedText = "changed text";
        private static readonly Guid APersistedId = Guid.NewGuid();
        private readonly MessageSqLiteRepository repository;
        private readonly ExampleDbContext exampleDbContext;

        public MessageRepositoryShould()
        {
            var options = new DbContextOptionsBuilder<ExampleDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
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
        public async Task store_a_message()
        {
            var expectedMessage = GivenNewMessage();

            await repository.Save(expectedMessage);

            var actualMessage = exampleDbContext.MessageRecord.Single();
            Assert.Equal(expectedMessage.Text, actualMessage.Text);
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
            await exampleDbContext.MessageRecord.AddAsync(messageRecord);
            await exampleDbContext.SaveChangesAsync();
            return new Message(APersistedId, SomeText);
        }

        public void Dispose()
        {
            exampleDbContext?.Dispose();
        }
    }
}
