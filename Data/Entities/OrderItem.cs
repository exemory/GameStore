namespace Data.Entities;

public class OrderItem : EntityBase
{
    public int Quantity { get; set; }
    
    public Guid GameId { get; set; }
    public Game Game { get; set; } = default!;
    
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;
}