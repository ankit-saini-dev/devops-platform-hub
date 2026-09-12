namespace DevOpsPlatformHub.Application.Exceptions;

public class ResourceConflictException() : Exception("The requested resource conflicts with the current state.")
{
}
