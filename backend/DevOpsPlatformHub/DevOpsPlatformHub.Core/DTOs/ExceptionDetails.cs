namespace DevOpsPlatformHub.Core.DTOs;

public sealed record ExceptionDetails(
    string ExceptionType,
    string Message,
    string? StackTrace,
    IReadOnlyList<ExceptionDetails> InnerExceptions);
