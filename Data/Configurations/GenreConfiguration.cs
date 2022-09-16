using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasIndex(g => g.Name)
            .IsUnique();
        
        builder.HasCheckConstraint($"CK_${nameof(Genre)}_${nameof(Genre.ParentId)}", $"[{nameof(Genre.ParentId)}] != [{nameof(Genre.Id)}]");
    }
}