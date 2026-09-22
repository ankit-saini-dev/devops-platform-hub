namespace DevOpsPlatformHub.Application.Dtos.Authentication;

public sealed record CurrentUserResponse(
    Guid Id,
    string Username,
    string Name,
    IReadOnlyCollection<string> Roles
);
