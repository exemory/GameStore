namespace Business.DataTransferObjects;

public class OrderCreationDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string PaymentType { get; set; } = default!;
    public string? Comments { get; set; }
    
    public IEnumerable<OrderItemDto> Items { get; set; } = default!;
}