using Example.Model;
using NSubstitute;
using Xunit;

namespace Example.Application.Tests
{
    public class MessageProcessingServiceShould
    {
        private const string AMessage = "a message";
        private const string AnyMessage = "any message";
        private const int AMessageId = 0;
        private const int ANotPersistedId = 123;
        private const string SomeText = "Some Text";

        private readonly IMessageRepository repository;
        private readonly IEventWriter bus;

        public MessageProcessingServiceShould()
        {
            repository = Substitute.For<IMessageRepository>();
            bus = Substitute.For<IEventWriter>();
        }

        [Fact]
        public void save_message_after_processing_it()
        {
            var expectedMessage = Substitute.For<Message>(new object[] { AMessageId, AMessage });
            repository.GetById(AMessageId).Returns(expectedMessage);

            new MessageProcessingService(repository, bus).Process(AMessageId, AnyMessage);

            expectedMessage.Received().Process(AnyMessage);
            repository.Received().Save(expectedMessage);
        }


        [Fact]
        public void publish_events_to_bus_when_saving_changes()
        {
            repository.GetById(ANotPersistedId).Returns(new Message(ANotPersistedId, SomeText));

            new MessageProcessingService(repository, bus).Process(ANotPersistedId, SomeText);

            bus.Received().Publish(Arg.Is<MatchingMessageReceived>(e =>
                e.MessageId == ANotPersistedId)); ;
        }
    }
}
