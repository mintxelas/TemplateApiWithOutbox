using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Application;
using Template.Application.Behaviors;
using Template.Application.CreateMessage;
using Template.Domain;
using Template.Infrastructure;
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

        public static IServiceCollection AddOutboxSupport<TOutboxDbContext, TOutboxRepository>(
            this IServiceCollection services, string connectionString, long outboxReadDueSeconds, long outboxReadPeriodSeconds)
            where TOutboxDbContext : DbContext, IOutboxDbContext
            where TOutboxRepository : class, IOutboxRepository
        {
            services.AddDbContext<IOutboxDbContext, TOutboxDbContext>(
                (serviceProvider, optionsBuilder) =>
                {
                    optionsBuilder.UseSqlite(connectionString);
                }, ServiceLifetime.Singleton);
            services.AddSingleton<IOutboxRepository, TOutboxRepository>();
            services.AddSingleton(_ => new RepeatingTimer(outboxReadDueSeconds * 1000, outboxReadPeriodSeconds * 1000));
            services.AddSingleton<IEventReader, BusSubscriptionsWithOutbox>();
            return services;
        }

        public static IServiceCollection AddSubscriptions(this IServiceCollection services)
        {
            services.AddTransient<NotificationsContextSubscriptions>();
            services.AddTransient<MonitoringContextSubscriptions>();
            return services;
        }

        public static IServiceCollection AddExampleRepository(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ExampleDbContext>((serviceProvider, optionsBuilder) =>
            {
                optionsBuilder.UseSqlite(connectionString);
            });
            services.AddScoped<IMessageRepository, MessageRepository>();
            return services;
        }

        public static IServiceCollection AddMediatorWithBehaviors(this IServiceCollection services)
        {
            services.AddMediatR(typeof(CreateMessageResponse).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));
            services.Scan(scan => scan
                .FromAssemblyOf<CreateMessageValidator>()
                .AddClasses(@class => @class.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces());
            return services;
        }
    }
}
