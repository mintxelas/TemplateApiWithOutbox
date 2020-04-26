using Template.Domain;

namespace Template.Application.CreateMessage
{
    public class CreateMessageResponse
    {
        public string Description { get; }

        public CreateMessageResponse(string description)
        {
            Description = description;
        }
    }

    public class CreateMessageSuccessResponse: CreateMessageResponse
    {
        public Message Message { get; }

        public CreateMessageSuccessResponse(Message message, string description): base(description)
        {
            Message = message;
        }
    }
}
