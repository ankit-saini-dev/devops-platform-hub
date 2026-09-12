using DevOpsPlatformHub.Api.Extension;
using DevOpsPlatformHub.Application.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;

namespace DevOpsPlatformHub.IntegrationTests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ErrorHandlingFixture : IAsyncLifetime
{
    private WebApplication? _application;
    public HttpClient HttpClient { get; private set; } = null!;
    public TestLogProvider LogProvider { get; } = new();

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(LogProvider);

        builder.Services.ConfigureServices();

        _application = builder.Build();
        _application.ConfigureApplication();

        _application.MapGet("/test/success", () => Results.Ok());
        _application.MapGet("/test/not-found", (Func<IResult>)(() => throw new ResourceNotFoundException()));
        _application.MapGet("/test/conflict", (Func<IResult>)(() => throw new ResourceConflictException()));
        _application.MapPost("/test/validation", (Func<IResult>)(() =>
                throw new ValidationFailureException(new Dictionary<string, string[]>
                {
                    ["name"] = ["Name is required."]
                })));
        _application.MapGet("/test/unexpected",
            (Func<IResult>)(() => throw new InvalidOperationException("Password=do-not-return-or-log-this-secret-value")));

        await _application.StartAsync();
        HttpClient = _application.GetTestClient();
        HttpClient.BaseAddress = new Uri("https://localhost");
    }

    public async Task DisposeAsync()
    {
        HttpClient.Dispose();
        if (_application is not null)
        {
            await _application.DisposeAsync();
        }
    }

    public void ClearLogs()
    {
        LogProvider.Clear();
    }
}
