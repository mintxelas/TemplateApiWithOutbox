using MediatR;
using System;

namespace Sample.Application.ProcessMessage
{
    public class ProcessMessageRequest: IRequest<ProcessMessageResponse>
    {
        public Guid MessageId { get; }
        public string TextToMatch { get; }

        public ProcessMessageRequest(Guid messageId, string textToMatch)
        {
            TextToMatch = textToMatch;
            MessageId = messageId;
        }
    }
}
