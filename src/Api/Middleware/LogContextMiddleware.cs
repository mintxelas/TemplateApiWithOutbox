using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Sample.Api.Middleware;

public class LogContextMiddleware(RequestDelegate next, ILogger<LogContextMiddleware> logger)
{
    public Task InvokeAsync(HttpContext context)
    {
        if (Activity.Current == null) return next(context);
        
        var correlationHeaders = Activity.Current
            .Baggage
            .Distinct()
            .ToDictionary(
                b => b.Key, 
                b => b.Value);

        using (logger.BeginScope(correlationHeaders))
        {
            return next(context);
        }
    }
}