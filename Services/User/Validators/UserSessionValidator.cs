using FluentValidation;
using Services.User.DTOs;

namespace Services.User.Validators;

public class UserSessionValidator : AbstractValidator<UserDtoLoginRequest>
{
    public UserSessionValidator()
    {
        RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email address");
        RuleFor(x => x.Password)
                .NotNull()
                .WithMessage("Password is required")
                .NotEmpty()
                .WithMessage("Password is required");
    }
}
