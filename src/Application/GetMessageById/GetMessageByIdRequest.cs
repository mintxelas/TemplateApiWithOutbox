using System;

namespace Sample.Application.GetMessageById;

public class GetMessageByIdRequest(Guid messageId) 
{
    public Guid MessageId { get; } = messageId;
}