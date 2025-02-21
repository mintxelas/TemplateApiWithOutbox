using System;

namespace Sample.Application.ProcessMessage;

public class ProcessMessageRequest(Guid messageId, string textToMatch)
{
    public Guid MessageId { get; } = messageId;
    public string TextToMatch { get; } = textToMatch;
}