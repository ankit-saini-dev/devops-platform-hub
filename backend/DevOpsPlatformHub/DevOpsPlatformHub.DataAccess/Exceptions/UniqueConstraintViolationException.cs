namespace DevOpsPlatformHub.DataAccess.Persistence.Exceptions;

public sealed class UniqueConstraintViolationException()
    : Exception("The database rejected a duplicate value.")
{
}
