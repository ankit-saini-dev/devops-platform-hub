using DevOpsPlatformHub.Application.Authentication;
using DevOpsPlatformHub.Application.Authentication.Contracts;
using DevOpsPlatformHub.Application.Dtos.Authentication;
using DevOpsPlatformHub.Application.Services;
using DevOpsPlatformHub.Core.Constants;
using DevOpsPlatformHub.Core.Exceptions;
using DevOpsPlatformHub.Entities.Entities;
using DevOpsPlatformHub.UnitTests.Authentication.Support;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DevOpsPlatformHub.UnitTests.Authentication.Services;

public class AuthenticationServiceTests
{
    [Fact]
    public async Task RegisterAsync_WithValidRequest_CreatesHashedUserAndIssuesToken()
    {
        // Arrange
        var testContext = CreateTestContext();
        var request = new RegistrationRequest(
            "Test User",
            "test-user",
            "test-user@example.com",
            "A-strong-password-123");

        // Act
        var result = await testContext.Service.RegisterAsync(request, CancellationToken.None);

        // Assert
        var user = Assert.Single(testContext.Repository.Users);

        Assert.NotEqual(request.Password, user.PasswordHash);
        Assert.Equal(PasswordVerificationResult.Success,
            testContext.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password));
        Assert.NotEmpty(result.AccessToken);
        Assert.Contains(RoleNameConstants.User, result.Roles);
        Assert.Equal(1, testContext.Repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ThrowsResourceConflictException()
    {
        // Arrange
        var testContext = CreateTestContext();
        testContext.Repository.Users.Add(CreateUser(testContext.PasswordHasher, "existing-user", "existing@example.com"));
        var request = new RegistrationRequest(
            "Another User",
            "existing-user",
            "another@example.com",
            "A-strong-password-123");

        // Act
        var action = () => testContext.Service.RegisterAsync(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ResourceConflictException>(action);
    }

    [Theory]
    [InlineData("existing-user")]
    [InlineData("existing@example.com")]
    public async Task AuthenticateAsync_WithUsernameOrEmail_ReturnsAccessToken(
        string identifier)
    {
        // Arrange
        var testContext = CreateTestContext();
        testContext.Repository.Users.Add(CreateUser(testContext.PasswordHasher, "existing-user", "existing@example.com"));
        var request = new LoginRequest(identifier, "A-strong-password-123");

        // Act
        var result = await testContext.Service.AuthenticateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.Equal("existing-user", result.Username);
        Assert.Contains(RoleNameConstants.User, result.Roles);
    }

    [Fact]
    public async Task AuthenticateAsync_WithIncorrectPassword_ReturnsNull()
    {
        // Arrange
        var testContext = CreateTestContext();
        testContext.Repository.Users.Add(CreateUser(testContext.PasswordHasher, "existing-user", "existing@example.com"));
        var request = new LoginRequest("existing-user", "incorrect-password");

        // Act
        var result = await testContext.Service.AuthenticateAsync(request, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    private static TestContext CreateTestContext()
    {
        var repository = new FakeIdentityRepository();
        var passwordHasher = new PasswordHasher<User>();

        repository.Roles.Add(new Role
        {
            Id = Guid.NewGuid(),
            Name = RoleNameConstants.User,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var tokenIssuer = new JwtTokenIssuer(Options.Create(new JwtOptions
        {
            Issuer = "unit-tests",
            Audience = "unit-tests",
            IssuerSigningKey = new string('a', 64),
            AccessTokenLifetimeMinutes = 10
        }));

        return new TestContext(new AuthenticationService(repository, passwordHasher, tokenIssuer), repository, passwordHasher);
    }

    private static User CreateUser(IPasswordHasher<User> passwordHasher, string username, string email)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = RoleNameConstants.User,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Username = username,
            Email = email,
            PasswordHash = string.Empty,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        user.PasswordHash = passwordHasher.HashPassword(user, "A-strong-password-123");
        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            User = user,
            Role = role
        });

        return user;
    }

    private sealed record TestContext(
        AuthenticationService Service,
        FakeIdentityRepository Repository,
        IPasswordHasher<User> PasswordHasher);
}
