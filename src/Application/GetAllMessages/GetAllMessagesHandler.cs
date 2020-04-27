using System.Linq;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Template.Domain;

namespace Template.Application.GetAllMessages
{
    public class GetAllMessagesHandler : IRequestHandler<GetAllMessagesRequest, GetAllMessagesResponse>
    {
        private readonly IMessageRepository repository;

        public GetAllMessagesHandler(IMessageRepository repository)
        {
            this.repository = repository;
        }

        public async Task<GetAllMessagesResponse> Handle(GetAllMessagesRequest request, CancellationToken cancellationToken) 
            => new GetAllMessagesResponse(await repository.GetAll());
    }
}