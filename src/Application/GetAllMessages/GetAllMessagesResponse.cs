using Sample.Domain;

namespace Sample.Application.GetAllMessages;

public class GetAllMessagesResponse(Message[] messages)
{
    public Message[] Messages { get; } = messages;
}