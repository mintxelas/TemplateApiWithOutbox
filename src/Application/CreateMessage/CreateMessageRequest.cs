using MediatR;

namespace Template.Application.CreateMessage
{
    public class CreateMessageRequest: IRequest<CreateMessageResponse>
    {
        public string Text { get; }

        public CreateMessageRequest(string text)
        {
            Text = text;
        }
    }
}
