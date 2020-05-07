using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using Template.Infrastructure.Configuration;

namespace Template.Api.Middleware
{
    public class ValidateConfigurationStartupFilter : IStartupFilter
    {
        private readonly IEnumerable<IValidateConfiguration> configurationObjects;

        public ValidateConfigurationStartupFilter(IEnumerable<IValidateConfiguration> configurationObjects)
        {
            this.configurationObjects = configurationObjects;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            foreach (var configuration in configurationObjects)
            {
                configuration.Validate();
            }

            return next;
        }
    }
}
