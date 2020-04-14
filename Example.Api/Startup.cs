using Example.Application;
using Example.Infrastructure;
using Example.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Example.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<InMemoryBus>();
            services.AddSingleton<IEventWriter>(
                serviceProvider => serviceProvider.GetRequiredService<InMemoryBus>());
            services.AddSingleton<IEventReader>(
                serviceProvider => serviceProvider.GetRequiredService<InMemoryBus>());
            services.AddSingleton<NotificationsContextSubscriptions>();
            services.AddSingleton<MonitoringContextSubscriptions>();

            services.AddScoped<MessageProcessingService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
            NotificationsContextSubscriptions notificationsContext, MonitoringContextSubscriptions monitoringContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
