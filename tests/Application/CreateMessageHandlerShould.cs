using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sample.Application.CreateMessage;
using Sample.Domain;
using Xunit;

namespace Sample.Application.Tests;

public class CreateMessageHandlerShould
{
    private const string SomeText = "some text";
    private const string SomeMessage = "some message";
    private static readonly Guid SomeGuid = Guid.NewGuid();
    private readonly IMessageRepository repository;
    private readonly CreateMessageHandler handler;

    public CreateMessageHandlerShould()
    {
        repository = Substitute.For<IMessageRepository>();
        handler = new CreateMessageHandler(repository);
    }

    [Fact]
    public async Task return_the_created_message_id_when_successful()
    {
        var givenMessage = new Message(SomeText);
        var expectedMessage = new Message(SomeGuid, SomeText);
        repository.Save(givenMessage).Returns(expectedMessage);
        var request = new CreateMessageRequest(SomeText);

        var response = await handler.Handle(request, CancellationToken.None);

        Assert.IsType<CreateMessageSuccessResponse>(response);
        Assert.Equal(expectedMessage, ((CreateMessageSuccessResponse)response).Message);
    }

    [Fact]
    public async Task return_a_generic_message_with_a_description_when_it_fails()
    {
        var givenMessage = new Message(SomeText);
        repository.Save(givenMessage).Throws(new Exception(SomeMessage));
        var request = new CreateMessageRequest(SomeText);

        var response = await handler.Handle(request, CancellationToken.None);

        Assert.IsNotType<CreateMessageSuccessResponse>(response);
        Assert.Equal(SomeMessage, response.Description);
    }
}