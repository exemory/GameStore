using System.Text.RegularExpressions;
using Business.DataTransferObjects;
using FluentValidation;

namespace Business.Validators;

public class GameCreationDtoValidator : AbstractValidator<GameCreationDto>
{
    public GameCreationDtoValidator()
    {
        RuleFor(g => g.Key)
            .NotEmpty()
            .Length(2, 20)
            .Matches("^[a-z0-9-]*$", RegexOptions.IgnoreCase)
            .WithMessage($"'{nameof(GameCreationDto.Key)}' must consist only of latin letters and dashes");

        RuleFor(g => g.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(g => g.Price)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1000);

        RuleFor(g => g.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(g => g.ImageFileName)
            .NotEmpty();
    }
}