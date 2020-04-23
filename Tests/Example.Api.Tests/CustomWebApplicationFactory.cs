using Example.Application;
using Example.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Example.Infrastructure.SqLite;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public IMessageRepository MessageRepository { get; set; }
        public MessageProcessingService MessageProcessingService { get; set; }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var exampleDbContextDescriptor =
                    services.SingleOrDefault(s => s.ServiceType == typeof(ExampleDbContext));
                services.Remove(exampleDbContextDescriptor);
                services.AddDbContext<ExampleDbContext>(optionsBuilder =>
                    optionsBuilder.UseInMemoryDatabase(this.GetType().Name));

                var outboxDbContextDescriptor =
                    services.SingleOrDefault(s => s.ServiceType == typeof(OutboxConsumerDbContext));
                services.Remove(outboxDbContextDescriptor);
                services.AddDbContext<OutboxConsumerDbContext>(optionsBuilder =>
                    optionsBuilder.UseInMemoryDatabase(this.GetType().Name), ServiceLifetime.Singleton);

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
