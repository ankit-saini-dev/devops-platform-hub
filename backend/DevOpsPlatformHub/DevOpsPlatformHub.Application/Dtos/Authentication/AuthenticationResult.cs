namespace DevOpsPlatformHub.Application.Dtos.Authentication;

public record AuthenticationResult(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string Name,
    string Username,
    IReadOnlyCollection<string> Roles);
