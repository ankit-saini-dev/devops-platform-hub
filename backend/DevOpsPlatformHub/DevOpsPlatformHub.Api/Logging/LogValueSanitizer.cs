namespace DevOpsPlatformHub.Api.Logging;

internal static class LogValueSanitizer
{
    /// <summary>
    /// Removes carriage-return and line-feed characters from a value before it is written to a log.
    /// </summary>
    /// <param name="value">The value to sanitize.</param>
    /// <returns>The value without carriage-return or line-feed characters.</returns>
    public static string Sanitize(string value)
    {
        return value
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty);
    }
}
