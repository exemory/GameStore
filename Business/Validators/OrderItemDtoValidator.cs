using Business.DataTransferObjects;
using FluentValidation;

namespace Business.Validators;

public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(i => i.Quantity)
            .GreaterThan(0);
    }
}