﻿using Business.DataTransferObjects;
using FluentValidation;

namespace Business.Validators
{
    public class SignUpDtoValidator : AbstractValidator<SignUpDto>
    {
        public SignUpDtoValidator()
        {
            RuleFor(d => d.Username)
                .NotEmpty()
                .Length(3, 15);

            RuleFor(d => d.Email)
                .NotNull()
                .EmailAddress();

            RuleFor(d => d.FirstName)
                .NotEmpty()
                .Length(1, 50);

            RuleFor(d => d.LastName)
                .NotEmpty()
                .Length(1, 50);

            RuleFor(d => d.Password)
                .NotNull()
                .MinimumLength(8);
        }
    }
}