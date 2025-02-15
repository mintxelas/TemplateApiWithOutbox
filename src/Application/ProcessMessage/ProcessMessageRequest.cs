using MediatR;
using System;

namespace Sample.Application.ProcessMessage;

public class ProcessMessageRequest(Guid messageId, string textToMatch) : IRequest<ProcessMessageResponse>
{
    public Guid MessageId { get; } = messageId;
    public string TextToMatch { get; } = textToMatch;
}