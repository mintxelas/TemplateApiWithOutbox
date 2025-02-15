using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Sample.Domain;

namespace Sample.Application.ProcessMessage
{
    public class ProcessMessageHandler : IRequestHandler<ProcessMessageRequest, ProcessMessageResponse>
    {
        private readonly IMessageRepository repository;

        public ProcessMessageHandler(IMessageRepository repository)
        {
            this.repository = repository;
        }

        public async Task<ProcessMessageResponse> Handle(ProcessMessageRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var messageToProcess = await repository.GetById(request.MessageId);
                if (messageToProcess is null) return new MessageToProcessNotFoundResponse($"Message with Id {request.MessageId} not found.");
                messageToProcess.Process(request.TextToMatch);
                var processedMessage = await repository.Save(messageToProcess);
                return new ProcessMessageResponse("Message processed.");
            }
            catch (Exception e)
            {
                return new ErrorProcessingMessageResponse(e.Message);
            }
        }
    }
}
