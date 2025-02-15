using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Application.Behaviors;

public class LoggingPipelineBehavior<TRequest, TResponse>(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {@Request}", request);
        var stopWatch = Stopwatch.StartNew();
        try
        {
            var response = await next();
            stopWatch.Stop();
            logger.LogInformation(
                "Handled {RequestType}: {@Response} in {Duration}ms", typeof(TRequest).Name, response, stopWatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            stopWatch.Stop();
            logger.LogInformation(
                "Request {RequestType} threw exception: {@Exception} in {Duration}ms", typeof(TRequest).Name, ex, stopWatch.ElapsedMilliseconds);
            throw;
        }
    }
}