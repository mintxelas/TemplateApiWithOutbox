using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Api.HealthChecks
{
    public static class VersionHealthCheckExtension
    {
        public static IEndpointConventionBuilder MapHealthCheckWithVersion(this IEndpointRouteBuilder endpoints,
            string pattern)
        {
            var assemblyVersionString = typeof(Program).Assembly.GetName().Version?.ToString();
            return endpoints.MapHealthChecks(pattern, new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonSerializer.Serialize(new
                        {
                            status = report.Status.ToString(),
                            assemblyVersion = assemblyVersionString,
                            duration = report.TotalDuration.TotalMilliseconds,
                            info = report.Entries.Select(e => new
                            {
                                e.Key,
                                e.Value.Description,
                                Status = e.Value.Status.ToString(),
                                Error = e.Value.Exception?.Message
                            }).ToArray()
                        },
                        new JsonSerializerOptions
                        {
                            IgnoreNullValues = true,
                            WriteIndented = true
                        }
                    );
                    await context.Response.WriteAsync(result);
                }
            });
        }
    }
}
