using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Template.Api.HealthChecks;
using Template.Api.Middleware;
using Template.Infrastructure.EntityFramework;

namespace Template.Api
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMediatorWithBehaviors();
            services.AddVersionedApi(defaultApiVersion: 1);
            services.AddSwaggerWithVersions("Template Api", 1, 2);
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
}
