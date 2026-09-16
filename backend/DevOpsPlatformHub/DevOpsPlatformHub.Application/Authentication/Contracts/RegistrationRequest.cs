namespace DevOpsPlatformHub.Application.Authentication.Contracts;

public sealed record RegistrationRequest(
    string Name,
    string Username,
    string Email,
    string Password);
