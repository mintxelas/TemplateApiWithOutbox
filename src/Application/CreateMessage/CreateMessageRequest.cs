using MediatR;

namespace Sample.Application.CreateMessage;

public class CreateMessageRequest(string text) : IRequest<CreateMessageResponse>
{
    public string Text { get; } = text;
}