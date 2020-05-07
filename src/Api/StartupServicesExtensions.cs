using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Template.Api.Middleware;
using Template.Application;
using Template.Application.Behaviors;
using Template.Domain;
using Template.Infrastructure.Configuration;
using Template.Infrastructure.EntityFramework;
using Template.Infrastructure.Repositories;
using Template.Infrastructure.Subscriptions;

namespace Template.Api
{
    public static class StartupServicesExtensions
    {
        public static IServiceCollection AddVersionedApi(this IServiceCollection services, int defaultApiVersion = 1)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(defaultApiVersion, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("X-version"),
                    new QueryStringApiVersionReader("api-version"));
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
            });

            return services;
        }

        public static IServiceCollection AddSwaggerWithVersions(this IServiceCollection services, 
            string apiDescription, int minVersion = 1, int maxVersion = 1)
        {
            return services.AddSwaggerGen(options =>
            {
                foreach (var version in Enumerable.Range(minVersion, (maxVersion - minVersion + 1)))
                {
                    options.SwaggerDoc($"v{version}", new Microsoft.OpenApi.Models.OpenApiInfo { Title = apiDescription, Version = $"v{version}" });
                }
            });
        }

        public static IServiceCollection AddOutboxSupport(
            this IServiceCollection services)
        {
            services.AddDbContext<MessageDbContext>(
                (serviceProvider, optionsBuilder) =>
                {
                    var connectionString = serviceProvider.GetRequiredService<OutBoxConfiguration>().Database;
                    optionsBuilder.UseSqlite(connectionString);
                });
            services.AddDbContext<IOutboxDbContext, OutboxConsumerDbContext>(
                (serviceProvider, optionsBuilder) =>
                {
                    var connectionString = serviceProvider.GetRequiredService<OutBoxConfiguration>().Database;
                    optionsBuilder.UseSqlite(connectionString);
                }, ServiceLifetime.Singleton);
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddSingleton<IOutboxRepository, OutboxRepository>();
            services.AddSingleton<RepeatingTimer>();
            services.AddSingleton<IEventReader, BusSubscriptionsWithOutbox>();
            return services;
        }

        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configurationSection)
        {
            services.Configure<OutBoxConfiguration>(configurationSection);
            services.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<OutBoxConfiguration>>().Value);
            services.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<OutBoxConfiguration>>().Value.Timer);
            return services;
        }

        public static IServiceCollection AddSubscriptions(this IServiceCollection services)
        {
            services.AddTransient<NotificationsContextSubscriptions>();
            services.AddTransient<MonitoringContextSubscriptions>();
            return services;
        }

        public static IServiceCollection AddMediatorWithBehaviors(this IServiceCollection services)
        {
            services.AddMediatR(typeof(Placeholder).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));
            services.Scan(scan => scan
                .FromAssemblyOf<Placeholder>()
                .AddClasses(@class => @class.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces());
            return services;
        }

        public static void AddConfigurationValidation(this IServiceCollection services)
        {
            services.AddTransient<IStartupFilter, ValidateConfigurationStartupFilter>();
            services.AddSingleton<IEnumerable<IValidateConfiguration>>(serviceProvider =>
                new IValidateConfiguration[]
                {
                    serviceProvider.GetRequiredService<OutBoxConfiguration>(),
                    serviceProvider.GetRequiredService<TimerConfiguration>()
                });
        }
    }
}
