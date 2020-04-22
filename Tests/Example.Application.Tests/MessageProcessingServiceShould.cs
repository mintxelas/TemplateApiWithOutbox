using Example.Domain;
using NSubstitute;
using Xunit;

namespace Example.Application.Tests
{
    public class MessageProcessingServiceShould
    {
        private const string AMessage = "a message";
        private const string AnyMessage = "any message";
        private const int AMessageId = 0;
        private readonly IMessageRepository repository;

        public MessageProcessingServiceShould()
        {
            repository = Substitute.For<IMessageRepository>();
        }

        [Fact]
        public void save_message_after_processing_it()
        {
            var expectedMessage = Substitute.For<Message>(new object[] { AMessageId, AMessage });
            repository.GetById(AMessageId).Returns(expectedMessage);

            new MessageProcessingService(repository).Process(AMessageId, AnyMessage);

            expectedMessage.Received().Process(AnyMessage);
            repository.Received().Save(expectedMessage);
        }
    }
}
