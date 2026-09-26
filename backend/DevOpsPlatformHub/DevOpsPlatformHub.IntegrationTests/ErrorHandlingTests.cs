using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace DevOpsPlatformHub.IntegrationTests;

/// <summary>
/// Verifies the HTTP error responses produced by the test host's middleware pipeline.
/// </summary>
public sealed class ErrorHandlingTests(ErrorHandlingFixture fixture) : IClassFixture<ErrorHandlingFixture>
{
    /// <summary>
    /// Returns an unprocessable response when the request fails input validation.
    /// </summary>
    [Fact]
    public async Task PostValidation_WhenValidationFails_ReturnsUnprocessableEntityProblemDetails()
    {
        // Arrange
        fixture.ClearLogs();

        // Act
        using var response = await fixture.HttpClient.PostAsync("/test/validation", null);
        var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("Validation failed.", document.RootElement.GetProperty("title").GetString());
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
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
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
        Assert.Equal("The requested resource conflicts with the current state.", document.RootElement.GetProperty("title").GetString());
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    /// <summary>
    /// Returns safe problem details for an unhandled server exception.
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
        Assert.DoesNotContain("do-not-return-or-log-this-secret-value", body);
        Assert.DoesNotContain("Exception", body);

        var errorLog = Assert.Single(fixture.LogProvider.Entries,
            entry => entry.CategoryName.EndsWith("GlobalExceptionHandler") && entry.LogLevel == LogLevel.Error);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

}
