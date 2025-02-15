using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Sample.Domain;

namespace Sample.Application.GetMessageById
{
    public class GetMessageByIdHandler : IRequestHandler<GetMessageByIdRequest, GetMessageByIdResponse>
    {
        private readonly IMessageRepository repository;

        public GetMessageByIdHandler(IMessageRepository repository)
        {
            this.repository = repository;
        }

        public async Task<GetMessageByIdResponse> Handle(GetMessageByIdRequest request, CancellationToken cancellationToken)
        {
            var message = await repository.GetById(request.MessageId);
            if (message is null)
                return new MessageByIdNotFoundResponse();
            return new GetMessageByIdResponse(message);
        }
    }
}