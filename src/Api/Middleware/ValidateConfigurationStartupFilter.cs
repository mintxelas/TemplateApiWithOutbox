using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Sample.Infrastructure.Configuration;

namespace Sample.Api.Middleware;

public class ValidateConfigurationStartupFilter(IEnumerable<IValidateConfiguration> configurationObjects)
    : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        foreach (var configuration in configurationObjects)
        {
            configuration.Validate();
        }

        return next;
    }
}