using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Sample.Api.HealthChecks;

public static class CustomHealthCheckExtensions
{
    public static IEndpointConventionBuilder MapHealthCheckWithVersion(this IEndpointRouteBuilder endpoints,
        string pattern)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

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
                    }, jsonSerializerOptions);
                await context.Response.WriteAsync(result);
            }
        });
    }

    public static IHealthChecksBuilder AddSelfCheck(this IHealthChecksBuilder builder)
    {
        return builder.AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
    }
    
    public static IEndpointConventionBuilder MapLivenessProbe(this IEndpointRouteBuilder endpoints, string pattern)
    {
        return endpoints.MapHealthChecks(pattern, new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("live")
        });
    }
}