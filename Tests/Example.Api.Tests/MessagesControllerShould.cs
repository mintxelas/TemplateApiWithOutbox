using Example.Model;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Example.Api.Tests
{
    public class MessagesControllerShould : IClassFixture<CustomWebApplicationFactory<Example.Api.Startup>>
    {
        private const int SomeMessageId = 123;
        private readonly CustomWebApplicationFactory<Startup> factory;

        public MessagesControllerShould(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            var mockBusReader = Substitute.For<IEventReader>();
            var mockBusWriter = Substitute.For<IEventWriter>();
            var mockRepository = Substitute.For<IMessageRepository>();
            factory.BusReader = mockBusReader;
            factory.BusWriter = mockBusWriter;
            factory.MessageRepository = mockRepository;
        }        

        [Fact]
        public async Task initialize_subscribers_and_publish_events_on_matching_post()
        {
            factory.MessageRepository
                .GetById(SomeMessageId)
                .Returns(new Message(SomeMessageId, "Hello"));
            var client = factory.CreateClient();

            var response = await client.PostAsync($"/messages/{SomeMessageId}", null);

            response.EnsureSuccessStatusCode(); 
            factory.BusReader.Received(2)
                .Subscribe(Arg.Any<Action<MatchingMessageReceived>>());
            factory.BusWriter.Received()
                .Publish(Arg.Is<MatchingMessageReceived>(e => e.MessageId == SomeMessageId));
        }
    }
}
