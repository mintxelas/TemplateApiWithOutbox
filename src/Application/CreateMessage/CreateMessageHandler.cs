using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Template.Domain;

namespace Template.Application.CreateMessage
{
    public class CreateMessageHandler : IRequestHandler<CreateMessageRequest, CreateMessageResponse>
    {
        private readonly IMessageRepository repository;

        public CreateMessageHandler(IMessageRepository repository)
        {
            this.repository = repository;
        }

        public async Task<CreateMessageResponse> Handle(CreateMessageRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var messageToCreate = new Message(request.Text);
                var createdMessage = await repository.Save(messageToCreate);
                return new CreateMessageSuccessResponse(createdMessage, "Message successfully created.");
            }
            catch (Exception e)
            {
                return new CreateMessageResponse(e.Message);
            }
        }
    }
}
