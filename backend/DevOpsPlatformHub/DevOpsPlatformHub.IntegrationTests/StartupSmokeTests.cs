using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DevOpsPlatformHub.IntegrationTests;

/// <summary>
/// Verifies that the application starts successfully and exposes its baseline endpoints.
/// </summary>
public class StartupSmokeTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// Returns the OpenAPI document as a successful JSON response after application startup.
    /// </summary>
    [Fact]
    public async Task GetOpenApiDocument_WhenApplicationStarts_ReturnsSuccessfulJsonResponse()
    {
        // Arrange
        var clientOptions = new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        };
        using var client = factory.CreateClient(clientOptions);

        // Act
        using var response = await client.GetAsync("/openapi/v1.json");
        var responseMediaType = response.Content.Headers.ContentType?.MediaType;

        // Assert
        Assert.Equal("application/json", responseMediaType);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
