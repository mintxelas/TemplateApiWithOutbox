using MediatR;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Template.Api.Models;
using Template.Application.CreateMessage;
using Template.Application.ProcessMessage;
using Template.Domain;
using Xunit;

namespace Template.Api.Tests
{
    public class MessagesControllerShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private const string SomeText = "some text";
        private const string AnyDescription = "any description";
        private static readonly Guid SomeId = Guid.NewGuid();
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly HttpClient client;

        public MessagesControllerShould(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            factory.MessageRepository = Substitute.For<IMessageRepository>();
            factory.ProcessMessageHandler =
                Substitute.For<IRequestHandler<ProcessMessageRequest, ProcessMessageResponse>>();
            factory.CreateMessageHandler =
                Substitute.For<IRequestHandler<CreateMessageRequest, CreateMessageResponse>>();
            client = this.factory.CreateClient();
        }

        [Fact]
        public async Task return_all_repository_messages_on_get()
        {
            var expectedMessage = new Message(SomeId, SomeText);
            factory.MessageRepository
                .GetAll()
                .Returns(new [] {expectedMessage});

            var response = await client.GetAsync("/messages");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actualMessages = JsonSerializer.Deserialize<MessageDto[]>(responseString);
            Assert.Contains(expectedMessage.Id, actualMessages.Select(m => m.id));
        }

        [Fact]
        public async Task return_the_corresponding_message_given_an_id_on_get()
        {
            factory.MessageRepository
                    .GetById(SomeId)
                    .Returns(new Message(SomeId, SomeText));

            var response = await client.GetAsync($"/messages/{SomeId}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actualMessage = JsonSerializer.Deserialize<MessageDto>(responseString);
            Assert.Equal(SomeId, actualMessage.id);
            Assert.Equal(SomeText, actualMessage.text);
        }

        [Fact]
        public async Task create_a_message_on_post()
        {
            factory.CreateMessageHandler
                .Handle(Arg.Any<CreateMessageRequest>(), Arg.Any<CancellationToken>())
                .Returns(new CreateMessageSuccessResponse(new Message(SomeId, SomeText), AnyDescription));
            var requestContent = new FormUrlEncodedContent(new [] {
                new KeyValuePair<string, string>("text", SomeText)
            });

            var response = await client.PostAsync("/messages", requestContent);

            response.EnsureSuccessStatusCode();
            await factory.CreateMessageHandler.Received()
                .Handle(Arg.Is<CreateMessageRequest>(r => r.Text == SomeText), Arg.Any<CancellationToken>());
        }

        [Fact]
        public Task use_processing_service_on_put_v1() 
            => use_processing_service_on_put("1.0", "Hello");

        [Fact]
        public Task use_processing_service_on_put_v2() 
            => use_processing_service_on_put("2.0", "World");

        private async Task use_processing_service_on_put(string version, string matchingWord)
        {
            var givenRequest = new ProcessMessageRequest(SomeId, matchingWord);
            factory.ProcessMessageHandler
                .Handle(Arg.Any<ProcessMessageRequest>(), Arg.Any<CancellationToken>())
                .Returns(new ProcessMessageResponse(AnyDescription));
            client.DefaultRequestHeaders.Add("X-version", version);

            var response = await client.PutAsync($"/messages/process/{SomeId}", null);

            response.EnsureSuccessStatusCode();
            await factory.ProcessMessageHandler.Received()
                .Handle(Arg.Is<ProcessMessageRequest>(
                    r => r.MessageId == givenRequest.MessageId && r.TextToMatch == givenRequest.TextToMatch), Arg.Any<CancellationToken>());
        }   
    }
}
