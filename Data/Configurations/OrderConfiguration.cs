using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.FirstName)
            .HasMaxLength(50);
        
        builder.Property(o => o.LastName)
            .HasMaxLength(50);
        
        builder.Property(o => o.Email)
            .HasMaxLength(256);
        
        builder.Property(o => o.Phone)
            .HasMaxLength(20);
        
        builder.Property(o => o.Comments)
            .HasMaxLength(600);
    }
}