using System.Threading;
using System.Threading.Tasks;
using Sample.Domain;

namespace Sample.Application.GetAllMessages;

public class GetAllMessagesHandler(IMessageRepository repository)
    : IRequestHandler<GetAllMessagesRequest, GetAllMessagesResponse>
{
    public async Task<GetAllMessagesResponse> Handle(GetAllMessagesRequest request, CancellationToken cancellationToken = default) 
        => new GetAllMessagesResponse(await repository.GetAll());
}