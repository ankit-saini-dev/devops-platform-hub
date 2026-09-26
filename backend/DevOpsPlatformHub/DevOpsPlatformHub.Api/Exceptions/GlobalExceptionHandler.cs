using DevOpsPlatformHub.Core.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsPlatformHub.Api.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = ExceptionMapping.GetStatusCode(exception);
        switch (statusCode)
        {
            case StatusCodes.Status400BadRequest:
            case StatusCodes.Status409Conflict:
            case StatusCodes.Status422UnprocessableEntity:
                LogMessage.ValidationFailed(logger, exception.Message);
                break;
            case StatusCodes.Status404NotFound:
                logger.LogWarning("Resource not found during {RequestMethod} {RequestPath}: {Message}",
                    httpContext.Request.Method.SanitizeValue(), httpContext.Request.Path.ToString().SanitizeValue(), exception.Message);
                break;
            default:
                if (statusCode >= StatusCodes.Status500InternalServerError)
                {
                    LogMessage.UnhandledException(logger, exception, httpContext.Request.Method.SanitizeValue(),
                        httpContext.Request.Path.ToString().SanitizeValue());
                }

                break;
        }

        var clientMessage = ExceptionMapping.GetClientMessage(exception);
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = clientMessage,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        if (ExceptionMapping.ExposeExceptionMessageInResponse(statusCode))
        {
            problemDetails.Detail = clientMessage;
            problemDetails.Extensions["message"] = clientMessage;
        }
        else if (environment.IsDevelopment() && statusCode >= StatusCodes.Status500InternalServerError)
        {
            problemDetails.Detail = exception.Message;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
