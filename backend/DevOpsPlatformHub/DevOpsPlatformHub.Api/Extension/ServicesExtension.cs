using DevOpsPlatformHub.Api.ErrorHandling;
using DevOpsPlatformHub.Infrastructure.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DevOpsPlatformHub.Api.Extension;

public static class ServicesExtension
{
    extension(IServiceCollection services)
    {
        public void ConfigureServices()
        {
            services.AddInfrastructureServices();
            services.ConfigureHealthChecks();
        }

        private void AddInfrastructureServices()
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
    }
}
