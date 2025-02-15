using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Sample.Api.Middleware
{
    public class LogContextMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LogContextMiddleware> logger;

        public LogContextMiddleware(RequestDelegate next, ILogger<LogContextMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public Task InvokeAsync(HttpContext context)
        {
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
}
