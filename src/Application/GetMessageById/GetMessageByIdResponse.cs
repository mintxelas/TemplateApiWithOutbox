﻿using Template.Domain;

namespace Template.Application.GetMessageById
{
    public class GetMessageByIdResponse
    {
        public Message Message { get; }

        public GetMessageByIdResponse(Message message)
        {
            Message = message;
        }
    }

    public class MessageByIdNotFoundResponse : GetMessageByIdResponse
    {
        public MessageByIdNotFoundResponse(): base(null)
        {
            
        }
    }
}