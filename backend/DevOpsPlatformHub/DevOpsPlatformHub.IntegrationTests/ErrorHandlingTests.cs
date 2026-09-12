using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace DevOpsPlatformHub.IntegrationTests;

/// <summary>
/// Verifies the HTTP error responses and request logging produced by the test host's middleware pipeline.
/// </summary>
public sealed class ErrorHandlingTests(ErrorHandlingFixture fixture) : IClassFixture<ErrorHandlingFixture>
{
    /// <summary>
    /// Returns validation problem details when the request fails input validation.
    /// </summary>
    [Fact]
    public async Task PostValidation_WhenValidationFails_ReturnsBadRequestProblemDetails()
    {
        // Arrange
        fixture.ClearLogs();

        // Act
        using var response = await fixture.HttpClient.PostAsync("/test/validation", null);
        var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("validation_failed", document.RootElement.GetProperty("code").GetString());
        Assert.True(document.RootElement.GetProperty("errors").TryGetProperty("name", out _));
        Assert.True(document.RootElement.TryGetProperty("traceId", out _));
    }

    /// <summary>
    /// Returns resource-not-found problem details when the requested resource does not exist.
    /// </summary>
    [Fact]
    public async Task GetNotFound_WhenResourceDoesNotExist_ReturnsNotFoundProblemDetails()
    {
        // Arrange
        fixture.ClearLogs();

        // Act
        using var response = await fixture.HttpClient.GetAsync("/test/not-found");
        var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("resource_not_found", document.RootElement.GetProperty("code").GetString());
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    /// <summary>
    /// Returns resource-conflict problem details when the request conflicts with the current state.
    /// </summary>
    [Fact]
    public async Task GetConflict_WhenRequestConflicts_ReturnsConflictProblemDetails()
    {
        // Arrange
        fixture.ClearLogs();

        // Act
        using var response = await fixture.HttpClient.GetAsync("/test/conflict");
        var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("resource_conflict", document.RootElement.GetProperty("code").GetString());
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    /// <summary>
    /// Returns safe problem details and prevents sensitive exception data from being exposed or logged.
    /// </summary>
    [Fact]
    public async Task GetUnexpected_WhenUnhandledExceptionOccurs_ReturnsSafeProblemDetails()
    {
        // Arrange
        fixture.ClearLogs();

        // Act
        using var response = await fixture.HttpClient.GetAsync("/test/unexpected");
        var body = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(body);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("unexpected_error", document.RootElement.GetProperty("code").GetString());
        Assert.DoesNotContain("do-not-return-or-log-this-secret-value", body);
        Assert.DoesNotContain("InvalidOperationException", body);

        var errorLog = Assert.Single(fixture.LogProvider.Entries,
            entry => entry.CategoryName.EndsWith("GlobalExceptionHandler") && entry.LogLevel == LogLevel.Error);
        var exceptionDetails = Assert.Single(errorLog.Properties,
            property => property.Key == "ExceptionDetails").Value;
        var serializedExceptionDetails = JsonSerializer.Serialize(exceptionDetails);

        Assert.DoesNotContain("do-not-return-or-log-this-secret-value", serializedExceptionDetails);
        Assert.Contains("[REDACTED]", serializedExceptionDetails);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    /// <summary>
    /// Logs structured request metadata at information level for a successful request without an error log.
    /// </summary>
    [Fact]
    public async Task GetSuccess_WhenRequestSucceeds_LogsStructuredRequestWithoutError()
    {
        // Arrange
        fixture.ClearLogs();

        // Act
        using var response = await fixture.HttpClient.GetAsync("/test/success");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var requestLog = Assert.Single(fixture.LogProvider.Entries, entry => entry.CategoryName.EndsWith("RequestLoggingMiddleware"));

        Assert.Equal(LogLevel.Information, requestLog.LogLevel);
        Assert.Contains(requestLog.Properties, property => property.Key == "RequestMethod");
        Assert.Contains(requestLog.Properties, property => property.Key == "RequestPath");
        Assert.Contains(requestLog.Properties, property => property.Key == "StatusCode");
        Assert.Contains(requestLog.Properties, property => property.Key == "ElapsedMilliseconds");
        Assert.Contains(requestLog.Properties, property => property.Key == "TraceId");
        Assert.DoesNotContain(fixture.LogProvider.Entries, entry => entry.LogLevel == LogLevel.Error);
    }

    /// <summary>
    /// Does not create a request log entry for the liveness endpoint, which is excluded from
    /// application request logging.
    /// </summary>
    [Fact]
    public async Task GetLiveness_WhenHealthEndpointIsCalled_DoesNotWriteRequestLog()
    {
        // Arrange
        fixture.ClearLogs();

        // Act
        using var response = await fixture.HttpClient.GetAsync("/health/live");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(fixture.LogProvider.Entries,
            entry => entry.CategoryName.EndsWith("RequestLoggingMiddleware"));
    }
}
