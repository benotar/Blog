using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Persistence.EntityTypeConfiguration;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.ToTable("Likes");

        builder.HasKey(like => new { like.CommentId, like.UserId });

        builder.HasOne(like => like.Comment)
            .WithMany(comment => comment.Likes)
            .HasForeignKey(like => like.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}