using Business.DataTransferObjects;
using Data.Enums;
using FluentValidation;

namespace Business.Validators;

public class OrderCreationDtoValidator : AbstractValidator<OrderCreationDto>
{
    public OrderCreationDtoValidator()
    {
        RuleFor(d => d.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(d => d.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(d => d.Email)
            .NotNull()
            .EmailAddress();

        RuleFor(d => d.Phone)
            .NotEmpty()
            .Matches(@"^\+\d{11,16}$").WithMessage("Phone number is invalid.");

        RuleFor(o => o.PaymentType)
            .NotEmpty()
            .IsEnumName(typeof(PaymentType), false);

        RuleFor(o => o.Items)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(i => i.Any()).WithMessage("'{PropertyName}' must not be empty.");

        RuleForEach(o => o.Items)
            .NotNull()
            .SetValidator(new OrderItemDtoValidator());
    }
}