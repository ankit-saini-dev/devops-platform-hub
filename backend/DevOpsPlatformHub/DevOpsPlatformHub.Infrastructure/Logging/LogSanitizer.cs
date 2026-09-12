using System.Text.RegularExpressions;

namespace DevOpsPlatformHub.Infrastructure.Logging;

public static partial class LogSanitizer
{
    private const int MaximumExceptionDepth = 5;
    private static readonly Regex KeyValueSecretPattern = KeyValueSecretPatternRegex();
    private static readonly Regex AuthorizationSecretPattern = AuthorizationSecretPatternRegex();
    private static readonly Regex PostgreSqlUriSecretPattern = PostgreSqlUriSecretPatternRegex();

    /// <summary>
    /// Removes carriage-return and line-feed characters from a value before it is written to a log.
    /// </summary>
    /// <param name="value">The value to sanitize.</param>
    /// <returns>The value without carriage-return or line-feed characters.</returns>
    public static string SanitizeValue(string value)
    {
        return value
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty);
    }

    public static ExceptionDetails SanitizeException(Exception exception)
    {
        return CreateExceptionDetails(exception, 0);
    }

    private static ExceptionDetails CreateExceptionDetails(Exception exception, int depth)
    {
        if (depth == MaximumExceptionDepth)
        {
            return new ExceptionDetails(
                "ExceptionChainTruncated",
                "The inner exception exceeded the supported logging depth.",
                null,
                []);
        }

        var innerException = exception switch
        {
            AggregateException aggregateException => aggregateException.InnerExceptions
                .Select(innerException => CreateExceptionDetails(innerException, depth + 1))
                .ToArray(),
            { InnerException: not null } => [CreateExceptionDetails(exception.InnerException, depth + 1)],
            _ => Array.Empty<ExceptionDetails>()
        };

        return new ExceptionDetails(
            exception.GetType().FullName ?? exception.GetType().Name,
            SanitizeExceptionText(exception.Message) ?? string.Empty,
            SanitizeExceptionText(exception.StackTrace),
            innerException);
    }

    private static string? SanitizeExceptionText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var sanitizedText = SanitizeValue(text);
        sanitizedText = KeyValueSecretPattern.Replace(sanitizedText, "$1$2[REDACTED]");
        sanitizedText =  AuthorizationSecretPattern.Replace(sanitizedText, "Authorization$1[REDACTED]");
        sanitizedText = PostgreSqlUriSecretPattern.Replace(sanitizedText, "$1[REDACTED]@");

        return sanitizedText;
    }

    public sealed record ExceptionDetails(
        string ExceptionType,
        string Message,
        string? StackTrace,
        IReadOnlyList<ExceptionDetails> InnerExceptions);

    [GeneratedRegex(
        """(?i)\b(password|pwd|secret|token|api[-_]?key|access[-_]?token|refresh[-_]?token|client[-_]?secret)\b\s*(=|:)\s*("[^"]*"|'[^']*'|[^;,\s]+)""",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex KeyValueSecretPatternRegex();

    [GeneratedRegex(@"(?i)\bauthorization\b\s*(=|:)\s*[^\r\n]+", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex AuthorizationSecretPatternRegex();

    [GeneratedRegex(@"(?i)((?:postgres|postgresql)://[^:/\s]+:)[^@/\s]+@", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex PostgreSqlUriSecretPatternRegex();
}
