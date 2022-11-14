namespace Business.DataTransferObjects;

public class OrderItemDto
{
    public Guid GameId { get; set; }
    public int Quantity { get; set; }
}