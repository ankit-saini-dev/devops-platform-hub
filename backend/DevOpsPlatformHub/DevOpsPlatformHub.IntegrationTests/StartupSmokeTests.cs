using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DevOpsPlatformHub.IntegrationTests;

public class StartupSmokeTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetOpenApiDocument_WhenApplicationStarts_ReturnsSuccessfulJsonResponse()
    {
        var clientOptions = new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        };
        using var client = factory.CreateClient(clientOptions);
        using var response = await client.GetAsync("/openapi/v1.json");
        var responseMediaType = response.Content.Headers.ContentType?.MediaType;
        Assert.Equal("application/json", responseMediaType);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
