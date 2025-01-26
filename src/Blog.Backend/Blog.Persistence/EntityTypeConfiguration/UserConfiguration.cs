using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Persistence.EntityTypeConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(user => user.Username)
            .IsRequired();
        
        builder.Property(user => user.Email)
            .IsRequired();

        builder.Property(user => user.ProfilePictureUrl)
            .IsRequired()
            .HasDefaultValue("https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_960_720.png");
        
        builder.HasIndex(user => new { user.Username, user.Email })
            .IsUnique();
    }
}