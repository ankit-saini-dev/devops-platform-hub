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
                LogValueSanitizer.Sanitize(httpContext.Request.Method),
                LogValueSanitizer.Sanitize(httpContext.Request.Path),
                httpContext.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                httpContext.TraceIdentifier);
        }
    }
}
