using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Template.Api.HealthChecks;
using Template.Application;
using Template.Infrastructure.SqLite;

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
            services.AddVersionedApi();
            services.AddSwaggerWithVersions("Template Api", 1, 2);
            services.AddOutboxSupport<OutboxConsumerDbContext, OutboxSqLiteRepository>(
                connectionString: @"Data Source=MessagesDB.db",
                outboxReadDueSeconds: 10, 
                outboxReadPeriodSeconds: 10);
            services.AddSubscriptions();
            services.AddScoped<MessageProcessingService>();
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
        }
    }
}
