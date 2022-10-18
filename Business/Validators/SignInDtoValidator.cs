using Business.DataTransferObjects;
using FluentValidation;

namespace Business.Validators
{
    public class SignInDtoValidator : AbstractValidator<SignInDto>
    {
        public SignInDtoValidator()
        {
            RuleFor(d => d.Login)
                .NotEmpty();
            
            RuleFor(d => d.Password)
                .NotNull();
        }
    }
}