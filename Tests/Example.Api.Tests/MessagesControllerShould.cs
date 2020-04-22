using Example.Api.Models;
using Example.Application;
using Example.Model;
using NSubstitute;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Example.Api.Tests
{
    public class MessagesControllerShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private const int SomeId = 123;
        private const string SomeText = "some text";
        private readonly CustomWebApplicationFactory<Startup> factory;

        public MessagesControllerShould(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            var mockRepository = Substitute.For<IMessageRepository>();
            factory.MessageRepository = mockRepository;
            var mockService = Substitute.For<MessageProcessingService>(new object[] { null, null });
            factory.MessageProcessingService = mockService;
        }

        [Fact]
        public async Task return_the_corresponding_message_given_an_id_on_get()
        {
            factory.MessageRepository
                    .GetById(SomeId)
                    .Returns(new Message(SomeId, SomeText));
            var client = factory.CreateClient();

            var response = await client.GetAsync($"/messages/{SomeId}");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var actualMessage = JsonSerializer.Deserialize<MessageDto>(responseString);
            Assert.Equal(SomeId, actualMessage.id);
            Assert.Equal(SomeText, actualMessage.text);
        }

        [Fact]
        public Task use_processing_service_on_post_v1() 
            => use_processing_service_on_post("1.0", "Hello");

        [Fact]
        public Task use_processing_service_on_post_v2() 
            => use_processing_service_on_post("2.0", "World");

        private async Task use_processing_service_on_post(string version, string matchingWord)
        {
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-version", version);

            var response = await client.PostAsync($"/messages/{SomeId}", null);

            response.EnsureSuccessStatusCode();
            factory.MessageProcessingService.Received().Process(SomeId, matchingWord);  
        }   
    }
}
