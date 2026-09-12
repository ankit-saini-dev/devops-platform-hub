using DevOpsPlatformHub.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace DevOpsPlatformHub.Infrastructure.HealthChecks;

public sealed class PostgresReadinessHealthCheck(IConfiguration configuration, ILogger<PostgresReadinessHealthCheck> logger)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        var connectionString = configuration.GetConnectionString("PlatformDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return HealthCheckResult.Unhealthy("PostgreSQL is unavailable");
        }

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand("SELECT 1;", connection);
            await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception exception)
        {
            logger.LogWarning("PostgreSQL readiness check failed. ExceptionDetails: {ExceptionDetails}",
                LogSanitizer.SanitizeException(exception));

            return HealthCheckResult.Unhealthy("PostgreSQL is unavailable");
        }
    }
}
