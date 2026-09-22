namespace DevOpsPlatformHub.Core.Exceptions;

public class ResourceConflictException() : Exception("The requested resource conflicts with the current state.")
{
    public string? Errors { get; set; }
}
