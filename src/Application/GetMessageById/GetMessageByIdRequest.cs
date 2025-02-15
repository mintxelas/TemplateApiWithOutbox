using MediatR;
using System;

namespace Sample.Application.GetMessageById
{
    public class GetMessageByIdRequest : IRequest<GetMessageByIdResponse>
    {
        public Guid MessageId { get; }

        public GetMessageByIdRequest(Guid messageId)
        {
            MessageId = messageId;
        }
    }
}