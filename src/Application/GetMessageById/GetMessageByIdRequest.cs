using MediatR;
using System;

namespace Sample.Application.GetMessageById;

public class GetMessageByIdRequest(Guid messageId) : IRequest<GetMessageByIdResponse>
{
    public Guid MessageId { get; } = messageId;
}