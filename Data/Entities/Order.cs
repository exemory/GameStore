using Data.Enums;

namespace Data.Entities;

public class Order : EntityBase
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public PaymentType PaymentType { get; set; }
    public string? Comments { get; set; }

    public IEnumerable<OrderItem> Items { get; set; } = default!;
}