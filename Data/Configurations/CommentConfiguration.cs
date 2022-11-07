using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(c => c.Body)
            .HasMaxLength(600);

        builder.HasCheckConstraint($"CK_{nameof(Comment)}_{nameof(Comment.ParentId)}",
            $"[{nameof(Comment.ParentId)}] != [{nameof(Comment.Id)}]");

        builder.Property(c => c.CreationDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.Deleted)
            .HasDefaultValue(false);
    }
}