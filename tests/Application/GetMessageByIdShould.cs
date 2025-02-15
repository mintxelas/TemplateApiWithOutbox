using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sample.Application.GetAllMessages;
using Sample.Application.GetMessageById;
using Sample.Domain;
using Xunit;

namespace Sample.Application.Tests
{
    public class GetMessageByIdShould
    {
        private readonly IMessageRepository repository;
        private readonly GetMessageByIdHandler handler;
        private static readonly Guid SomeId = Guid.NewGuid();
        private const string SomeText = "some text";

        public GetMessageByIdShould()
        {
            repository = Substitute.For<IMessageRepository>();
            handler = new GetMessageByIdHandler(repository);
        }

        [Fact]
        public async Task return_not_found_when_no_matching_message_is_found()
        {
            var response = await handler.Handle(new GetMessageByIdRequest(SomeId), CancellationToken.None);
            Assert.IsType<MessageByIdNotFoundResponse>(response);
        }

        [Fact]
        public async Task return_the_message_that_matches_the_given_id()
        {
            var givenMessage = new Message(SomeId, SomeText);
            repository.GetById(SomeId).Returns(givenMessage);
            var response = await handler.Handle(new GetMessageByIdRequest(SomeId), CancellationToken.None);
            Assert.Equal(givenMessage, response.Message);
        }
    }
}