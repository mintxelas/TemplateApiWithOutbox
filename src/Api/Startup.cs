using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Api.HealthChecks;
using Sample.Api.Middleware;
using Sample.Infrastructure.EntityFramework;

namespace Sample.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddMediatorWithBehaviors();
        services.AddVersionedApi(defaultApiVersion: 1);
        services.AddSwaggerWithVersions("Sample Api", 1, 2);
        services.AddCustomConfiguration(configuration.GetSection("OutBox"));
        services.AddConfigurationValidation();
        services.AddOutboxSupport();
        services.AddSubscriptions();
        services.AddHealthChecks()
            .AddDbContextCheck<MessageDbContext>();
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
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthCheckWithVersion("/health");
        });

        app.UseSubscriptions();
        app.UseMiddleware<LogContextMiddleware>();
    }
}