using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
     private const int PricePrecision = 6;
     private const int PriceScale = 2;
     private const decimal MinPrice = 0;
     private const decimal MaxPrice = 1000;
    
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.Property(g => g.Key)
            .HasMaxLength(20);

        builder.Property(g => g.Name)
            .HasMaxLength(50);
        
        builder.Property(g => g.Price)
            .HasPrecision(PricePrecision, PriceScale);

        builder.Property(g => g.Description)
            .HasMaxLength(2000);

        builder.HasIndex(g => g.Key)
            .IsUnique();
        
        builder.HasCheckConstraint($"CK_{nameof(Game)}_{nameof(Game.Price)}",
            $"[{nameof(Game.Price)}] BETWEEN {MinPrice} AND {MaxPrice}");
    }
}