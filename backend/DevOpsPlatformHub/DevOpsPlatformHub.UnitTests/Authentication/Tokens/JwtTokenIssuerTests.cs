using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevOpsPlatformHub.Domain.Constants;
using DevOpsPlatformHub.Domain.Entities;
using DevOpsPlatformHub.Infrastructure.Authentication;
using Microsoft.Extensions.Options;

namespace DevOpsPlatformHub.UnitTests.Authentication.Tokens;

public class JwtTokenIssuerTests
{
    [Fact]
    public void Issue_WithUserAndRoles_CreatesExpectedJwtClaims()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Username = "test-user",
            Email = "test-user@example.com",
            PasswordHash = "not-used-by-this-test",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var options = new JwtOptions
        {
            Issuer = "unit-tests",
            Audience = "unit-tests-client",
            IssuerSigningKey = new string('a', 64),
            AccessTokenLifetimeMinutes = 10
        };
        var tokenIssuer = new JwtTokenIssuer(Options.Create(options));

        // Act
        var result = tokenIssuer.Issue(user, [RoleNameConstants.User, RoleNameConstants.Operator]);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        // Assert
        Assert.Equal(options.Issuer, token.Issuer);
        Assert.Contains(options.Audience, token.Audiences);
        Assert.Equal(user.Id.ToString(), token.Subject);
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.NameIdentifier && claim.Value == user.Id.ToString());
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.Name && claim.Value == user.Username);
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.GivenName && claim.Value == user.Name);
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == RoleNameConstants.User);
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == RoleNameConstants.Operator);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal([RoleNameConstants.User, RoleNameConstants.Operator], result.Roles);
        Assert.True(result.ExpiresAt > DateTimeOffset.UtcNow);
    }
}
