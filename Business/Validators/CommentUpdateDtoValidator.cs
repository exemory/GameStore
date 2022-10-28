using Business.DataTransferObjects;
using FluentValidation;

namespace Business.Validators;

public class CommentUpdateDtoValidator : AbstractValidator<CommentUpdateDto>
{
    public CommentUpdateDtoValidator()
    {
        RuleFor(c => c.Body)
            .NotEmpty()
            .MaximumLength(600);
    }
}