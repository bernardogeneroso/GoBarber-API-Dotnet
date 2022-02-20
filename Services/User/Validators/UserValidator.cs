using FluentValidation;
using Services.User.DTOs;

namespace Services.User.Validators;

public class UserValidator : AbstractValidator<UserDtoRequest>
{
    public UserValidator()
    {
        RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email is invalid");
        RuleFor(x => x.DisplayName)
                .NotEmpty()
                .WithMessage("Display name is required");
        RuleFor(x => x.LatestPassword)
                .NotEmpty()
                .WithMessage("Latest password is required")
                .NotNull()
                .WithMessage("Latest password is required");
        RuleFor(x => x.Password);
        RuleFor(x => x.PasswordConfirmation)
                .Equal(x => x.Password)
                .WithMessage("Password confirmation must match password");
    }
}
