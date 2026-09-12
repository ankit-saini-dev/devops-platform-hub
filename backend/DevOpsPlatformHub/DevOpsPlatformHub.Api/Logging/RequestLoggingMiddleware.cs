using System.Diagnostics;

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
            logger.LogInformation(
                "Http request completed. {RequestMethod} {RequestPath} {StatusCode} {ElapsedMilliseconds} {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                httpContext.TraceIdentifier);
        }
    }
}
