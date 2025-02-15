using Sample.Domain;

namespace Sample.Application.CreateMessage;

public class CreateMessageResponse(string description)
{
    public string Description { get; } = description;
}

public class CreateMessageSuccessResponse(Message message, string description) : CreateMessageResponse(description)
{
    public Message Message { get; } = message;
}