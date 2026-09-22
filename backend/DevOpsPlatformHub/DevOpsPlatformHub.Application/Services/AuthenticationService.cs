using DevOpsPlatformHub.Application.Authentication.Contracts;
using DevOpsPlatformHub.Application.Authentication.Validations;
using DevOpsPlatformHub.Application.Dtos.Authentication;
using DevOpsPlatformHub.Core.Constants;
using DevOpsPlatformHub.Core.Exceptions;
using DevOpsPlatformHub.DataAccess.Persistence.Exceptions;
using DevOpsPlatformHub.DataAccess.Persistence.Repositories.Contracts;
using DevOpsPlatformHub.Entities.Entities;
using Microsoft.AspNetCore.Identity;

namespace DevOpsPlatformHub.Application.Services;

public class AuthenticationService(
    IIdentityRepository identityRepository,
    IPasswordHasher<User> passwordHasher,
    JwtTokenIssuer jwtTokenIssuer
) : IAuthenticationService
{
    public async Task<AuthenticationResult?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var user = await FindActiveUserAsync(request.Identifier, cancellationToken);
        if (user is null || !await VerifyPasswordAsync(user, request.Password, cancellationToken))
        {
            return null;
        }

        return jwtTokenIssuer.Issue(user, GetRoleName(user));
    }

    public async Task<AuthenticationResult> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken)
    {
        RegistrationRequestValidator.Validate(request);
        var name = request.Name.Trim();
        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim().ToLowerInvariant();
        if (await UserExistsAsync(username, email, cancellationToken))
        {
            throw new ResourceConflictException();
        }

        var defaultRole = await identityRepository.FindRoleByNameAsync(RoleNameConstants.User, cancellationToken)
                          ?? throw new InvalidOperationException($"The required {RoleNameConstants.User} role was not found.");

        var user = CreateUser(name, username, email, request.Password, defaultRole);
        identityRepository.AddUser(user);
        try
        {
            await identityRepository.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            throw new ResourceConflictException();
        }

        return jwtTokenIssuer.Issue(user, [defaultRole.Name]);
    }

    private async Task<User?> FindActiveUserAsync(string identifier, CancellationToken cancellationToken)
    {
        var trimmedIdentifier = identifier.Trim();
        var user = trimmedIdentifier.Contains('@', StringComparison.Ordinal)
            ? await identityRepository.FindUserByEmailWithRolesAsync(trimmedIdentifier, cancellationToken)
            : await identityRepository.FindUserByUsernameWithRolesAsync(trimmedIdentifier, cancellationToken);

        return user is { IsActive: true } ? user : null;
    }

    private async Task<bool> VerifyPasswordAsync(User user, string password, CancellationToken cancellationToken)
    {
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        switch (verificationResult)
        {
            case PasswordVerificationResult.Failed:
                return false;
            case PasswordVerificationResult.SuccessRehashNeeded:
                user.PasswordHash = passwordHasher.HashPassword(user, password);
                user.UpdatedAt = DateTimeOffset.UtcNow;

                await identityRepository.SaveChangesAsync(cancellationToken);
                break;
        }

        return true;
    }

    private async Task<bool> UserExistsAsync(string username, string email, CancellationToken cancellationToken)
    {
        var usernameUser = await identityRepository.FindUserByUsernameWithRolesAsync(username, cancellationToken);
        var emailUser = await identityRepository.FindUserByEmailWithRolesAsync(email, cancellationToken);

        return usernameUser is not null || emailUser is not null;
    }

    private User CreateUser(string name, string username, string email, string password, Role role)
    {
        var now = DateTimeOffset.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            CreatedAt = now,
            UpdatedAt = now,
            PasswordHash = string.Empty,
            Username = username,
            IsActive = true
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);
        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            User = user,
            Role = role
        });

        return user;
    }

    private IReadOnlyCollection<string> GetRoleName(User user)
    {
        return
        [
            .. user.UserRoles
                .Select(userRole => userRole.Role.Name)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(role => role, StringComparer.Ordinal)
        ];
    }
}
