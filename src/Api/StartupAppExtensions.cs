using System.Collections.Generic;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sample.Application.Subscriptions;

namespace Sample.Api
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
            var subscribers = app.ApplicationServices.GetService<IEnumerable<ISubscribeToContextEvents>>();
            foreach (var subscriber in subscribers)
            {
                subscriber.InitializeSubscriptions();
            }
            return app;
        }
    }
}
