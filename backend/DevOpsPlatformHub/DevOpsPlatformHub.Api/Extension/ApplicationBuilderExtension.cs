using DevOpsPlatformHub.Api.Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace DevOpsPlatformHub.Api.Extension;

public static class ApplicationBuilderExtension
{
    extension(WebApplication application)
    {
        public void ConfigureApplication()
        {
            application.ConfigureRequestPipeline();
            application.MapHealthChecks();
        }

        private void ConfigureRequestPipeline()
        {
            application.UseMiddleware<RequestLoggingMiddleware>();
            application.UseExceptionHandler();
            application.UseStatusCodePages();
            application.UseHttpsRedirection();
            application.MapControllers();
        }

        private void MapHealthChecks()
        {
            application.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = healthCheck => healthCheck.Tags.Contains("live")
            });

            application.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = healthCheck => healthCheck.Tags.Contains("ready")
            });
        }
    }
}
