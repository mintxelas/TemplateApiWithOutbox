using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Sample.Api.HealthChecks
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
                            duration = $"{report.TotalDuration.TotalMilliseconds} ms",
                            info = report.Entries.Select(e => new
                            {
                                name = e.Key,
                                description = e.Value.Description,
                                status = e.Value.Status.ToString(),
                                error = e.Value.Exception?.Message
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
