using DevOpsPlatformHub.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsPlatformHub.Api.ErrorHandling;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, errorCode) = exception switch
        {
            ValidationFailureException => (StatusCodes.Status400BadRequest, "Validation failed.", "validation_failed"),
            ResourceNotFoundException => (StatusCodes.Status404NotFound, "Resource not found.", "resource_not_found"),
            ResourceConflictException => (StatusCodes.Status409Conflict, "Request conflicts with the current state.", "resource_conflict"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", "unexpected_error")
        };

        logger.LogError(
            "Request failed with {ErrorCode}. ExceptionType: {ExceptionType}. ExceptionStackTrace: {ExceptionStackTrace}. TraceId: {TraceId}",
            errorCode,
            exception.GetType().FullName,
            exception.StackTrace,
            httpContext.TraceIdentifier);

        var problemDetails = exception is ValidationFailureException validationFailureException
            ? new ValidationProblemDetails(new Dictionary<string, string[]>(validationFailureException.Errors))
            : new ProblemDetails();
        problemDetails.Status = statusCode;
        problemDetails.Title = title;
        problemDetails.Extensions["code"] = errorCode;
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}
