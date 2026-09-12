namespace DevOpsPlatformHub.Api.Extension;

public static class ServicesExtension
{
    extension(IServiceCollection services)
    {
        public void ConfigureServices()
        {
            services.AddInfrastructureServices();
        }

        private void AddInfrastructureServices()
        {
            services.AddOpenApi();
            services.AddControllers();
        }
    }
}
