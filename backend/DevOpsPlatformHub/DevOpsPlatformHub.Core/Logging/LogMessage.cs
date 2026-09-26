using Microsoft.Extensions.Logging;

namespace DevOpsPlatformHub.Core.Logging;

public static partial class LogMessage
{
    [LoggerMessage(EventId = 190201, Level = LogLevel.Warning, Message = "Validation failed: {ValidationMessage}")]
    public static partial void ValidationFailed(ILogger logger, string validationMessage);

    [LoggerMessage(EventId = 190202, Level = LogLevel.Error, Message = "Unhandled exception during {RequestMethod} {RequestPath}")]
    public static partial void UnhandledException(ILogger logger, Exception exception, string requestMethod, string requestPath);
}
