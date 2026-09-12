using DevOpsPlatformHub.Api.Logging;

namespace DevOpsPlatformHub.Api.Extension;

public static class ApplicationBuilderExtension
{
    extension(WebApplication application)
    {
        public void ConfigureRequestPipeline()
        {
            application.UseMiddleware<RequestLoggingMiddleware>();
            application.UseExceptionHandler();
            application.UseStatusCodePages();
            application.UseHttpsRedirection();
            application.MapControllers();
        }
    }
}
