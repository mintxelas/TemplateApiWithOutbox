using Example.Application;
using Example.Domain;
using Example.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Example.Api
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

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("X-version"),
                    new QueryStringApiVersionReader("api-version"));
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Example API", Version = "v1" });
                options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Example API", Version = "v2" });
            });


            if (configuration["UseOutbox"] == "true")
            {
                services.AddSingleton<IOutboxRepository, OutboxInMemoryRepository>();
                services.AddSingleton<IEventReader, BusReaderWithOutbox>();
                services.AddScoped<IEventWriter>(
                    serviceProvider => serviceProvider.GetRequiredService<IOutboxRepository>());
            }
            else
            {
                services.AddSingleton<InMemoryBus>();
                services.AddSingleton<IEventWriter>(
                    serviceProvider => serviceProvider.GetRequiredService<InMemoryBus>());
                services.AddSingleton<IEventReader>(
                    serviceProvider => serviceProvider.GetRequiredService<InMemoryBus>());
            }

            services.AddSingleton<NotificationsContextSubscriptions>();
            services.AddSingleton<MonitoringContextSubscriptions>();

            services.AddScoped<MessageProcessingService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider,
            NotificationsContextSubscriptions notificationsContext, MonitoringContextSubscriptions monitoringContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            notificationsContext.InitializeSubscriptions();
            monitoringContext.InitializeSubscriptions();
        }
    }
}
