using DevOpsPlatformHub.Contexts;
using DevOpsPlatformHub.DataAccess.Persistence.Exceptions;
using DevOpsPlatformHub.DataAccess.Persistence.Repositories.Contracts;
using DevOpsPlatformHub.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DevOpsPlatformHub.DataAccess.Repositories;

public class IdentityRepository(PlatformDbContext dbContext) : IIdentityRepository
{
    public async Task<User?> FindUserByUsernameWithRolesAsync(string userName, CancellationToken cancellationToken)
    {
        var normalizedUsername = userName.Trim().ToLowerInvariant();

        return await dbContext.Users
            .Include(user => user.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .SingleOrDefaultAsync(
                user => user.Username.ToLower() == normalizedUsername,
                cancellationToken);
    }

    public async Task<User?> FindUserByEmailWithRolesAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await dbContext.Users
            .Include(user => user.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .SingleOrDefaultAsync(
                user => user.Email == normalizedEmail,
                cancellationToken);
    }

    public async Task<Role?> FindRoleByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        var normalizedRoleName = roleName.Trim().ToLowerInvariant();

        return await dbContext.Roles.SingleOrDefaultAsync(
            role => role.Name.ToLower() == normalizedRoleName,
            cancellationToken);
    }

    public void AddUser(User user)
    {
        dbContext.Users.Add(user);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new UniqueConstraintViolationException();
        }
    }
}
