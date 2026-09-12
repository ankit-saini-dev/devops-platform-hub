using System.Diagnostics;
using DevOpsPlatformHub.Infrastructure.Logging;

namespace DevOpsPlatformHub.Api.Logging;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next(httpContext);
        }
        finally
        {
            stopwatch.Stop();
            if (!httpContext.Request.Path.StartsWithSegments("/health"))
            {
                logger.LogInformation(
                    "Http request completed. {RequestMethod} {RequestPath} {StatusCode} {ElapsedMilliseconds} {TraceId}",
                    LogSanitizer.SanitizeValue(httpContext.Request.Method),
                    LogSanitizer.SanitizeValue(httpContext.Request.Path.Value ?? string.Empty),
                    httpContext.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    httpContext.TraceIdentifier);
            }
        }
    }
}
