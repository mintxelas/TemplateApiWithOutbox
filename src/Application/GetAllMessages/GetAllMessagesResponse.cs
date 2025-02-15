using Sample.Domain;

namespace Sample.Application.GetAllMessages
{
    public class GetAllMessagesResponse
    {
        public Message[] Messages { get; }

        public GetAllMessagesResponse(Message[] messages)
        {
            Messages = messages;
        }
    }
}