using DevOpsPlatformHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevOpsPlatformHub.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "platform");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(user => user.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(user => user.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
        builder.Property(user => user.Email).HasColumnName("email").HasMaxLength(254).IsRequired();
        builder.Property(user => user.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(user => user.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(user => user.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(user => user.IsActive).HasColumnName("is_active").IsRequired();
    }
}
