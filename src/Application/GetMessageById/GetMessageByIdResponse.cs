using Sample.Domain;

namespace Sample.Application.GetMessageById;

public class GetMessageByIdResponse(Message message)
{
    public Message Message { get; } = message;
}

public class MessageByIdNotFoundResponse() : GetMessageByIdResponse(null);