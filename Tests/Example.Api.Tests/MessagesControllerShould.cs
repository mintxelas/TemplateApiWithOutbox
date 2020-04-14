using Example.Api.Controllers;
using Example.Application;
using Example.Model;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Example.Api.Tests
{
    public class MessagesControllerShould
    {
        private const int AnyMessageId = 123;

        [Fact]
        public void relay_post_request_to_messages_service()
        {
            var repository = Substitute.For<IMessageRepository>();
            var service = Substitute.For<MessageProcessingService>(new[] { repository });
            var logger = Substitute.For<ILogger<MessagesController>>();
            var controller = new MessagesController(service, logger);

            controller.Post(AnyMessageId);

            service.Received().Process(AnyMessageId, Arg.Any<string>());            
        }
    }
}
