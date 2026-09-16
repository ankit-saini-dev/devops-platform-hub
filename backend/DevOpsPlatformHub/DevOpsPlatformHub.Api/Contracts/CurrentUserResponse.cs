namespace DevOpsPlatformHub.Api.Contracts;

public sealed record CurrentUserResponse(
    Guid Id,
    string Username,
    string Name,
    IReadOnlyCollection<string> Roles
);
