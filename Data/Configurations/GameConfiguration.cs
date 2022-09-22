using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.Property(g => g.Key)
            .HasMaxLength(20);

        builder.Property(g => g.Name)
            .HasMaxLength(50);

        builder.Property(g => g.Description)
            .HasMaxLength(2000);

        builder.HasIndex(g => g.Key)
            .IsUnique();
    }
}