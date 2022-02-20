using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.User.DTOs;

namespace Services.User.Validators;

public class UserCreateValidator : AbstractValidator<UserDtoCreateRequest>
{
    public UserCreateValidator(UserManager<AppUser> userManager)
    {
        RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email address")
                .MustAsync(async (email, cancellationToken) =>
                {
                    return !await userManager.Users.AnyAsync(x => x.Email == email, cancellationToken);
                })
                .WithMessage("Email already exists");
        RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required")
                .MustAsync(async (username, cancellationToken) =>
                {
                    return !await userManager.Users.AnyAsync(x => x.UserName == username, cancellationToken);
                })
                .WithMessage("Username already exists");
        RuleFor(x => x.DisplayName)
                .NotNull()
                .WithMessage("Display name is required")
                .NotEmpty()
                .WithMessage("Display name is required");
        RuleFor(x => x.IsBarber)
                .Must(ValidateBoolean)
                .WithMessage("Is barber must be true or false")
                .NotNull()
                .WithMessage("Is barber is required");
    }

    private bool ValidateBoolean(bool value)
    {
        return value == true || value == false;
    }
}
