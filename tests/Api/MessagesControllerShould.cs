using MediatR;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Sample.Api.Models;
using Sample.Application.CreateMessage;
using Sample.Application.GetAllMessages;
using Sample.Application.GetMessageById;
using Sample.Application.ProcessMessage;
using Sample.Domain;
using Xunit;

namespace Sample.Api.Tests;

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
        factory.GetAllMessagesHandler =
            Substitute.For<IRequestHandler<GetAllMessagesRequest, GetAllMessagesResponse>>();
        factory.GetMessageByIdHandler =
            Substitute.For<IRequestHandler<GetMessageByIdRequest, GetMessageByIdResponse>>();
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
        factory.GetAllMessagesHandler
            .Handle(Arg.Any<GetAllMessagesRequest>(), Arg.Any<CancellationToken>())
            .Returns(new GetAllMessagesResponse(new[] { expectedMessage }));

        var response = await client.GetAsync("/messages");

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var actualMessages = JsonSerializer.Deserialize<MessageDto[]>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.Contains(expectedMessage.Id, actualMessages.Select(m => m.Id));
    }

    [Fact]
    public async Task return_the_corresponding_message_given_an_id_on_get()
    {
        var expectedMessage = new Message(SomeId, SomeText);
        factory.GetMessageByIdHandler
            .Handle(Arg.Any<GetMessageByIdRequest>(), Arg.Any<CancellationToken>())
            .Returns(new GetMessageByIdResponse(expectedMessage));

        var response = await client.GetAsync($"/messages/{SomeId}");

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var actualMessage = JsonSerializer.Deserialize<MessageDto>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.Equal(expectedMessage.Id, actualMessage.Id);
        Assert.Equal(expectedMessage.Text, actualMessage.Text);
    }

    [Fact]
    public async Task return_not_found_when_service_does_not_find_message_on_get()
    {
        factory.GetMessageByIdHandler
            .Handle(Arg.Any<GetMessageByIdRequest>(), Arg.Any<CancellationToken>())
            .Returns(new MessageByIdNotFoundResponse());
        var response = await client.GetAsync($"/messages/{SomeId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task return_success_when_creating_a_message_on_post()
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
    public async Task return_bad_request_when_message_creation_fails()
    {
        factory.CreateMessageHandler
            .Handle(Arg.Any<CreateMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(new CreateMessageResponse(AnyDescription));
        var requestContent = new FormUrlEncodedContent(new[] {
            new KeyValuePair<string, string>("text", SomeText)
        });

        var response = await client.PostAsync("/messages", requestContent);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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

    [Fact]
    public Task return_not_found_when_processing_returns_not_found_v1() =>
        return_not_found_when_processing_returns_not_found("1.0");

    [Fact]
    public Task return_not_found_when_processing_returns_not_found_v2() =>
        return_not_found_when_processing_returns_not_found("2.0");

    private async Task return_not_found_when_processing_returns_not_found(string version)
    {
        factory.ProcessMessageHandler
            .Handle(Arg.Any<ProcessMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(new MessageToProcessNotFoundResponse(AnyDescription));
        client.DefaultRequestHeaders.Add("X-version", version);

        var response = await client.PutAsync($"/messages/process/{SomeId}", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public Task return_bad_request_when_processing_returns_error_v1() =>
        return_bad_request_when_processing_returns_error("1.0");

    [Fact]
    public Task return_bad_request_when_processing_returns_error_v2() =>
        return_bad_request_when_processing_returns_error("2.0");

    private async Task return_bad_request_when_processing_returns_error(string version)
    {
        factory.ProcessMessageHandler
            .Handle(Arg.Any<ProcessMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(new ErrorProcessingMessageResponse(AnyDescription));
        client.DefaultRequestHeaders.Add("X-version", version);

        var response = await client.PutAsync($"/messages/process/{SomeId}", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}