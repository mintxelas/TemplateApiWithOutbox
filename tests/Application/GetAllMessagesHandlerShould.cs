using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Template.Application.GetAllMessages;
using Template.Domain;
using Xunit;

namespace Template.Application.Tests
{
    public class GetAllMessagesHandlerShould
    {
        private readonly IMessageRepository repository;
        private readonly GetAllMessagesHandler handler;
        private static readonly Guid SomeId = Guid.NewGuid();
        private const string SomeText = "some text";

        public GetAllMessagesHandlerShould()
        {
            repository = Substitute.For<IMessageRepository>();
            handler = new GetAllMessagesHandler(repository);
        }

        [Fact]
        public async Task retrieve_an_empty_list_if_no_messages_exist()
        {
            var response = await handler.Handle(new GetAllMessagesRequest(), CancellationToken.None);
            Assert.Empty(response.Messages);
        }

        [Fact]
        public async Task get_all_database_items_as_list()
        {
            var givenMessages = new[] {new Message(SomeId, SomeText)};
            repository.GetAll().Returns(givenMessages);
            var response = await handler.Handle(new GetAllMessagesRequest(), CancellationToken.None);
            Assert.Equal(givenMessages, response.Messages);
        }
    }
}