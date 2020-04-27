using Template.Domain;

namespace Template.Application.GetAllMessages
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