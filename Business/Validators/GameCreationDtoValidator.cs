using Business.DataTransferObjects;
using FluentValidation;

namespace Business.Validators;

public class GameCreationDtoValidator : AbstractValidator<GameCreationDto>
{
    public GameCreationDtoValidator()
    {
        RuleFor(g => g.Key)
            .NotEmpty()
            .Length(2, 20);

        RuleFor(g => g.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(g => g.Description)
            .NotEmpty()
            .MaximumLength(2000);
    }
}