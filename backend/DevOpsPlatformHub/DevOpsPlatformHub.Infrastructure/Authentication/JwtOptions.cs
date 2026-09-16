using System.ComponentModel.DataAnnotations;

namespace DevOpsPlatformHub.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Authentication:Jwt";

    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    [Required]
    [MinLength(32)]
    public required string IssuerSigningKey { get; init; }

    [Range(1, 60)]
    public int AccessTokenLifetimeMinutes { get; init; }
}
