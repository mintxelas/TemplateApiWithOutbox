using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Application;
using Template.Domain;
using Template.Infrastructure;
using Template.Infrastructure.SqLite;

namespace Template.Api.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public IMessageRepository MessageRepository { get; set; }
        public MessageProcessingService MessageProcessingService { get; set; }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var inMemoryContextName = Guid.NewGuid().ToString();
            builder.ConfigureServices(services =>
            {
                var exampleDbContextDescriptor =
                    services.SingleOrDefault(s => s.ServiceType == typeof(ExampleDbContext));
                services.Remove(exampleDbContextDescriptor);
                services.AddDbContext<ExampleDbContext>(optionsBuilder =>
                    optionsBuilder.UseInMemoryDatabase(inMemoryContextName));

                var outboxDbContextDescriptor =
                    services.SingleOrDefault(s => s.ServiceType == typeof(IOutboxDbContext));
                services.Remove(outboxDbContextDescriptor);
                services.AddDbContext<IOutboxDbContext, OutboxConsumerDbContext>(optionsBuilder =>
                    optionsBuilder.UseInMemoryDatabase(inMemoryContextName), ServiceLifetime.Singleton);

                var repositoryDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IMessageRepository));
                services.Remove(repositoryDescriptor);
                services.AddScoped(_ => MessageRepository);

                var serviceDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(MessageProcessingService));
                services.Remove(serviceDescriptor);
                services.AddScoped(_ => MessageProcessingService);
            });
        }
    }
}
