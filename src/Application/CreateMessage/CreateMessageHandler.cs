using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Sample.Domain;

namespace Sample.Application.CreateMessage;

public class CreateMessageHandler(IMessageRepository repository)
    : IRequestHandler<CreateMessageRequest, CreateMessageResponse>
{
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