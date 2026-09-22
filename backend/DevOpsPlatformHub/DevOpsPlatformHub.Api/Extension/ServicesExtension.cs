using System.Security.Claims;
using System.Text;
using DevOpsPlatformHub.Api.ErrorHandling;
using DevOpsPlatformHub.Application.Authentication;
using DevOpsPlatformHub.Application.Authentication.Contracts;
using DevOpsPlatformHub.Application.Services;
using DevOpsPlatformHub.Contexts;
using DevOpsPlatformHub.DataAccess.HealthChecks;
using DevOpsPlatformHub.DataAccess.Persistence.Repositories.Contracts;
using DevOpsPlatformHub.DataAccess.Repositories;
using DevOpsPlatformHub.Entities.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

namespace DevOpsPlatformHub.Api.Extension;

public static class ServicesExtension
{
    extension(IServiceCollection services)
    {
        public void ConfigureServices(IConfiguration configuration)
        {
            services.AddApiServices();
            services.ConfigurePersistence(configuration);
            services.ConfigureAuthentication(configuration);
            services.ConfigureHealthChecks();
        }

        private void AddApiServices()
        {
            services.AddOpenApi();
            services.AddControllers();
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
        }

        private void ConfigureHealthChecks()
        {
            services.AddHealthChecks()
                .AddCheck("Api", () => HealthCheckResult.Healthy(), tags: ["live"])
                .AddCheck<PostgresReadinessHealthCheck>("postgresql", failureStatus: HealthStatus.Unhealthy, tags: ["ready"]);
        }

        private void ConfigurePersistence(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PlatformDatabase")
                                   ?? throw new InvalidOperationException("Connection string 'PlatformDatabase' is required.");

            services.AddDbContext<PlatformDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddSingleton<JwtTokenIssuer>();
        }

        private void ConfigureAuthentication(IConfiguration configuration)
        {
            var jwtSection = configuration.GetRequiredSection(JwtOptions.SectionName);
            var jwtOptions = jwtSection.Get<JwtOptions>()
                ?? throw new InvalidOperationException($"Configuration section '{JwtOptions.SectionName}' is required.");

            services.AddOptions<JwtOptions>()
                .Bind(jwtSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey)),
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            services.AddAuthorization();
        }
    }
}
