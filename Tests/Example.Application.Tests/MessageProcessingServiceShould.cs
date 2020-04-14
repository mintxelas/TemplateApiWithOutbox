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

        [Fact]
        public void save_message_after_processing_it()
        {
            var expectedMessage = Substitute.For<Message>(new object[] { AMessageId, AMessage });
            var repository = Substitute.For<IMessageRepository>();
            repository.GetById(AMessageId).Returns(expectedMessage);

            new MessageProcessingService(repository).Process(AMessageId, AnyMessage);

            expectedMessage.Received().Process(AnyMessage);
            repository.Received().Save(expectedMessage);
        }
    }
}
