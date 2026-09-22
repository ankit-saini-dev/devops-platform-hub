namespace DevOpsPlatformHub.Application.Dtos.Authentication;

public sealed record RegistrationRequest(
    string Name,
    string Username,
    string Email,
    string Password);
