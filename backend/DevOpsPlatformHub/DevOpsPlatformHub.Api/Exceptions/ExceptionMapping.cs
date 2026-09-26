using DevOpsPlatformHub.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsPlatformHub.Api.Exceptions;

public static class ExceptionMapping
{
    private const string DEFAULT_SERVER_ERROR_MESSAGE = "An error occured during processing your request";

    public static int GetStatusCode(Exception exception) => exception switch
    {
        KeyNotFoundException => StatusCodes.Status404NotFound,
        ArgumentException or BadHttpRequestException => StatusCodes.Status400BadRequest,
        ResourceConflictException => StatusCodes.Status409Conflict,
        InvalidOperationException => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status500InternalServerError
    };

    public static string GetClientMessage(Exception exception)
    {
        return ExposeExceptionMessageInResponse(GetStatusCode(exception))
            ? exception.Message
            : DEFAULT_SERVER_ERROR_MESSAGE;
    }

    public static bool ExposeExceptionMessageInResponse(int statusCode) =>
        statusCode is StatusCodes.Status404NotFound
            or StatusCodes.Status400BadRequest
            or StatusCodes.Status409Conflict
            or StatusCodes.Status422UnprocessableEntity;
}
