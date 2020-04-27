using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Template.Api.HealthChecks;
using Template.Api.Middleware;
using Template.Application;
using Template.Infrastructure.EntityFramework;
using Template.Infrastructure.Repositories;

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
            services.AddOutboxSupport<OutboxConsumerDbContext, OutboxRepository>(
                connectionString: @"Data Source=MessagesDB.db",
                outboxReadDueSeconds: 10, 
                outboxReadPeriodSeconds: 10);
            services.AddExampleRepository(@"Data Source=MessagesDB.db");
            services.AddSubscriptions();
            services.AddHealthChecks()
                .AddDbContextCheck<ExampleDbContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ExampleDbContext dbContext)
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
