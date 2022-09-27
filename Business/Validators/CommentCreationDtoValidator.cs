﻿using Business.DataTransferObjects;
using FluentValidation;

namespace Business.Validators;

public class CommentCreationDtoValidator : AbstractValidator<CommentCreationDto>
{
    public CommentCreationDtoValidator()
    {
        RuleFor(c => c.GameId)
            .NotEmpty();

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(c => c.Body)
            .NotEmpty()
            .MaximumLength(2000);
    }
}