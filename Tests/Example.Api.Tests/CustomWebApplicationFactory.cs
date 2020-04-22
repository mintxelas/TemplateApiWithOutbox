using Example.Application;
using Example.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

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
