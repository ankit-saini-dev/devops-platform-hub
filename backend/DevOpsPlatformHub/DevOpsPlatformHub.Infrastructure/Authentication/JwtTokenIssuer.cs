using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevOpsPlatformHub.Application.Authentication.Contracts;
using DevOpsPlatformHub.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DevOpsPlatformHub.Infrastructure.Authentication;

public sealed class JwtTokenIssuer(IOptions<JwtOptions> options)
{
    public AuthenticationResult Issue(User user, IReadOnlyCollection<string> roles)
    {
        var jwtOptions = options.Value;
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(jwtOptions.AccessTokenLifetimeMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.Name)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new AuthenticationResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            user.Name,
            user.Username,
            roles);
    }
}
