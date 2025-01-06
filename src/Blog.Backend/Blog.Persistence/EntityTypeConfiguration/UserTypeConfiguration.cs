using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Persistence.EntityTypeConfiguration;

public class UserTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(user => user.Username)
            .IsRequired();
        builder.Property(user => user.Email)
            .IsRequired();

        builder.HasIndex(user => new { user.Username, user.Email })
            .IsUnique();
    }
}