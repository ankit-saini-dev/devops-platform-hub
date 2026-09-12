namespace DevOpsPlatformHub.Application.Exceptions;

public class ValidationFailureException(IReadOnlyDictionary<string, string[]> errors)
    : Exception("One or more validation failure occurred")
{
    public IReadOnlyDictionary<string, string[]> Errors => errors;
}
