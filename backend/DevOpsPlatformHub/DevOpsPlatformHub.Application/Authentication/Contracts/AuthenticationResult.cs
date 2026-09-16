namespace DevOpsPlatformHub.Application.Authentication.Contracts;

public record AuthenticationResult(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string Name,
    string Username,
    IReadOnlyCollection<string> Roles);
