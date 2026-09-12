using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevOpsPlatformHub.IntegrationTests;

/// <summary>
/// Verifies the liveness and readiness endpoints against healthy and unavailable database states.
/// </summary>
public sealed class HealthCheckTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// Uses an unreachable PostgreSQL port so the test can exercise database-unavailable behavior
    /// without exposing a real connection string in the response.
    /// </summary>
    private const string UnavailableConnectionString = "Host=localhost;Port=1;Database=health_check_test;Username=test;Password=test";

    /// <summary>
    /// Liveness remains successful when the PostgreSQL dependency is unavailable.
    /// </summary>
    [Fact]
    public async Task GetLiveness_WhenPostgreSqlIsUnavailable_ReturnsOk()
    {
        // Arrange
        using var client = CreateClientWithUnavailableDatabase();

        // Act
        using var response = await client.GetAsync("/health/live");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Readiness reports service unavailability when the PostgreSQL dependency cannot be reached
    /// and does not include sensitive connection-string values in the response.
    /// </summary>
    [Fact]
    public async Task GetReadiness_WhenPostgreSqlIsUnavailable_ReturnsServiceUnavailable()
    {
        // Arrange
        using var client = CreateClientWithUnavailableDatabase();

        // Act
        using var response = await client.GetAsync("/health/ready");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.DoesNotContain("health_check_test", body);
        Assert.DoesNotContain("Username", body);
        Assert.DoesNotContain("Password", body);
    }

    /// <summary>
    /// Readiness succeeds when the test application's configured PostgreSQL dependency is available.
    /// </summary>
    [Fact]
    public async Task GetReadiness_WhenPostgreSqlIsAvailable_ReturnsOk()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        // Act
        using var response = await client.GetAsync("/health/ready");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Creates a test client whose application configuration points to an unavailable PostgreSQL instance.
    /// </summary>
    /// <returns>A client configured to use the unavailable database connection string.</returns>
    private HttpClient CreateClientWithUnavailableDatabase()
    {
        var configuredFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging => logging.ClearProviders());
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:PlatformDatabase"] = UnavailableConnectionString
                    });
            });
        });

        return configuredFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }
}
