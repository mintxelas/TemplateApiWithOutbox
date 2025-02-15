using System.Collections.Generic;
using System.Linq;
using Asp.Versioning;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sample.Api.Middleware;
using Sample.Application.Subscriptions;
using Sample.Domain;
using Sample.Infrastructure.Configuration;
using Sample.Infrastructure.EntityFramework;
using Sample.Infrastructure.Repositories;
using Sample.Infrastructure.Subscriptions;

namespace Sample.Api;

public static class StartupServicesExtensions
{
    public static IServiceCollection AddVersionedApi(this IServiceCollection services, int defaultApiVersion = 1)
    {
        services.AddApiVersioning(options =>
        {
            options.UnsupportedApiVersionStatusCode = 501;
            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(defaultApiVersion, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new HeaderApiVersionReader("X-version"),
                new QueryStringApiVersionReader("api-version"));
        }).AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            
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
        services.Scan(scan => scan
            .FromAssemblyOf<Application.Placeholder>()
            .AddClasses(@class => @class.AssignableTo<ISubscribeToContextEvents>())
            .AsImplementedInterfaces());
        return services;
    }

    public static IServiceCollection AddMediatorWithBehaviors(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Application.Placeholder).Assembly));
        services.Scan(scan => scan
            .FromAssemblyOf<Application.Placeholder>()
            .AddClasses(@class => @class.AssignableTo(typeof(IPipelineBehavior<,>)))
            .AsImplementedInterfaces());
        services.Scan(scan => scan
            .FromAssemblyOf<Application.Placeholder>()
            .AddClasses(@class => @class.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces());
        return services;
    }

    public static void AddConfigurationValidation(this IServiceCollection services)
    {
        services.AddTransient<IStartupFilter, ValidateConfigurationStartupFilter>();
        services.AddSingleton<IEnumerable<IValidateConfiguration>>(serviceProvider =>
        [
            serviceProvider.GetRequiredService<OutBoxConfiguration>(),
            serviceProvider.GetRequiredService<TimerConfiguration>()
        ]);
    }
}