using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Application;

namespace Template.Api
{
    public static class StartupAppExtensions
    {
        public static IApplicationBuilder UseSwaggerWithVersions(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });
            return app;
        }

        public static IApplicationBuilder UseSubscriptions(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetService<NotificationsContextSubscriptions>().InitializeSubscriptions();
            app.ApplicationServices.GetService<MonitoringContextSubscriptions>().InitializeSubscriptions();
            return app;
        }
    }
}
