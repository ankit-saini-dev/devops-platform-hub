using DevOpsPlatformHub.Application.Authentication.Contracts;
using DevOpsPlatformHub.Domain.Entities;

namespace DevOpsPlatformHub.UnitTests.Authentication.Support;

public class FakeIdentityRepository :  IIdentityRepository
{
    public List<User> Users { get; } = [];
    public List<Role> Roles { get; } = [];
    public int SaveChangesCallCount { get; private set; }

    public Task<User?> FindUserByUsernameWithRolesAsync(string username, CancellationToken cancellationToken)
    {
        var user = Users.SingleOrDefault(user =>
            string.Equals(user.Username, username.Trim(), StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task<User?> FindUserByEmailWithRolesAsync(string email, CancellationToken cancellationToken)
    {
        var user = Users.SingleOrDefault(user =>
            string.Equals(user.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task<Role?> FindRoleByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        var role = Roles.SingleOrDefault(role =>
            string.Equals(role.Name, roleName.Trim(), StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(role);
    }

    public void AddUser(User user)
    {
        Users.Add(user);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCallCount++;

        return Task.FromResult(1);
    }
}
