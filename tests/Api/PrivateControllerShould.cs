using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Api.Tests;

public class PrivateControllerShould(CustomWebApplicationFactory<Startup> factory)
    : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task RejectConnectingAnUnauthorizedRequest()
    {
        var response = await client.GetAsync("/private");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RespondAnAuthorizedRequest()
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "kjhlsjdhgsdhjfgjk");
        var response = await client.GetAsync("/private");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}