using Example.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Example.Api.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public IEventWriter BusWriter { get; set; }
        public IEventReader BusReader { get; set; }
        public IMessageRepository MessageRepository { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var writerDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IEventWriter));
                services.Remove(writerDescriptor);
                var readerDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IEventReader));
                services.Remove(readerDescriptor);
                var repositoryDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IMessageRepository));
                services.Remove(readerDescriptor);

                services.AddSingleton(BusWriter);
                services.AddSingleton(BusReader);
                services.AddScoped(_ => MessageRepository);
            });
        }
    }
}
