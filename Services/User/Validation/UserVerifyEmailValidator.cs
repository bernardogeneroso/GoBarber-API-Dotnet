using FluentValidation;
using Services.User.DTOs;

namespace Services.User.Validation;

public class UserVerifyEmailValidator : AbstractValidator<VerifyEmail.Command>
{
    public UserVerifyEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is invalid");
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required")
            .NotNull()
            .WithMessage("Token is required");
    }
}
