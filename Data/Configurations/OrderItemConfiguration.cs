using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasCheckConstraint($"CK_{nameof(OrderItem)}_{nameof(OrderItem.Quantity)}",
            $"[{nameof(OrderItem.Quantity)}] > 0");
    }
}