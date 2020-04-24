using System;
using System.Threading.Tasks;
using NSubstitute;
using Template.Domain;
using Xunit;

namespace Template.Application.Tests
{
    public class MessageProcessingServiceShould
    {
        private const string AMessage = "a message";
        private const string AnyMessage = "any message";
        private static readonly Guid AMessageId = Guid.NewGuid();
        private readonly IMessageRepository repository;

        public MessageProcessingServiceShould()
        {
            repository = Substitute.For<IMessageRepository>();
        }

        [Fact]
        public async Task save_message_after_processing_it()
        {
            var expectedMessage = Substitute.For<Message>(new object[] { AMessageId, AMessage });
            repository.GetById(AMessageId).Returns(expectedMessage);

            await new MessageProcessingService(repository).Process(AMessageId, AnyMessage);

            expectedMessage.Received().Process(AnyMessage);
            await repository.Received().Save(expectedMessage);
        }
    }
}
