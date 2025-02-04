using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Persistence.EntityTypeConfiguration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.HasOne(post => post.User).WithMany().HasForeignKey(post => post.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.UserId).IsRequired();

        builder.Property(p => p.Title).IsRequired();

        builder.Property(p => p.ImageUrl)
            .HasDefaultValue(
                "https://www.hostinger.com/tutorials/wp-content/uploads/sites/2/2021/09/how-to-write-a-blog-post.png");

        builder.Property(p => p.Category).HasDefaultValue(PostCategory.Uncategorized);

        builder.Property(p => p.Slug).IsRequired();

        builder.HasIndex(post => new { post.Title, post.Slug }).IsUnique();

        builder.HasIndex(post => new { post.Category, post.Slug, post.Title });
    }
}