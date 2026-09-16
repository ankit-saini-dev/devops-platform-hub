using DevOpsPlatformHub.Domain.Entities;

namespace DevOpsPlatformHub.Application.Authentication.Contracts;

public interface IIdentityRepository
{
    public Task<User?> FindUserByUsernameWithRolesAsync(string userName, CancellationToken cancellationToken);
    public Task<User?> FindUserByEmailWithRolesAsync(string email, CancellationToken cancellationToken);
    public Task<Role?> FindRoleByNameAsync(string roleName, CancellationToken cancellationToken);
    public void AddUser(User user);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
