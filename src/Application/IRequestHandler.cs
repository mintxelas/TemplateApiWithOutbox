using System.Threading;
using System.Threading.Tasks;

namespace Sample.Application;

public interface IRequestHandler<in TRequest, TResponse> where TRequest : notnull where TResponse : notnull
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}