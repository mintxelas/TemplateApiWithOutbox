using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Api.HealthChecks;
using Sample.Api.Middleware;
using Sample.Infrastructure.EntityFramework;

namespace Sample.Api;

public class Startup(IConfiguration configuration, IWebHostEnvironment environment)
{
    public void ConfigureServices(IServiceCollection services)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://localhost:5100";
                options.Audience = "internal-api";
                options.IncludeErrorDetails = environment.IsDevelopment();
            });
        services.AddControllers();
        services.AddMediatorWithBehaviors();
        services.AddVersionedApi(defaultApiVersion: 1);
        services.AddSwaggerWithVersions("Sample Api", 1, 2);
        services.AddCustomConfiguration(configuration.GetSection("OutBox"));
        services.AddConfigurationValidation();
        services.AddOutboxSupport();
        services.AddSubscriptions();
        services.AddHealthChecks()
            .AddDbContextCheck<MessageDbContext>()
            .AddSelfCheck();
        services.AddOpenTelemetryConfiguration(configuration);
        services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MessageDbContext dbContext)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        app.UseSwaggerWithVersions();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers().RequireAuthorization();
            endpoints.MapHealthCheckWithVersion("/health");
            endpoints.MapLivenessProbe("/alive");
        });

        app.UseSubscriptions();
        app.UseMiddleware<LogContextMiddleware>();
    }
}