using DevOpsPlatformHub.Application.Dtos.Authentication;

namespace DevOpsPlatformHub.Application.Authentication.Contracts;

public interface IAuthenticationService
{
    public Task<AuthenticationResult?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken);
    public Task<AuthenticationResult> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken);
}
