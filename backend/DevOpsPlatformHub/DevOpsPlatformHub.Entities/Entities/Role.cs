namespace DevOpsPlatformHub.Entities.Entities;

public class Role
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<UserRole> UserRoles { get; } = [];
}
