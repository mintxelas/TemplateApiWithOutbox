using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using MediatR;
using Template.Application.CreateMessage;
using Template.Application.ProcessMessage;
using Template.Domain;
using Template.Infrastructure.EntityFramework;

namespace Template.Api.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public IMessageRepository MessageRepository { get; set; }

        public IRequestHandler<CreateMessageRequest, CreateMessageResponse> CreateMessageHandler { get; set; }

        public IRequestHandler<ProcessMessageRequest, ProcessMessageResponse> ProcessMessageHandler { get; set; }

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

                var createMessageDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IRequestHandler<CreateMessageRequest, CreateMessageResponse>));
                services.Remove(createMessageDescriptor);
                services.AddScoped(_ => CreateMessageHandler);

                var processMessageDescriptor = services.SingleOrDefault(s =>
                    s.ServiceType == typeof(IRequestHandler<ProcessMessageRequest, ProcessMessageResponse>));
                services.Remove(processMessageDescriptor);
                services.AddScoped(_ => ProcessMessageHandler);
            });
        }
    }
}
