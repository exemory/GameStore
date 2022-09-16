using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasCheckConstraint($"CK_${nameof(Comment)}_${nameof(Comment.ParentId)}", $"[{nameof(Comment.ParentId)}] != [{nameof(Comment.Id)}]");
    }
}