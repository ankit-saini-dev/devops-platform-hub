using DevOpsPlatformHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevOpsPlatformHub.IntegrationTests.Authentication.Support;

public sealed class AuthenticationWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
    }

    public async Task DeleteUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        var normalizedUsername = username.Trim().ToLowerInvariant();
        var user = await dbContext.Users.SingleOrDefaultAsync(
            user => user.Username.ToLower() == normalizedUsername,
            cancellationToken);
        if (user is null)
        {
            return;
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
