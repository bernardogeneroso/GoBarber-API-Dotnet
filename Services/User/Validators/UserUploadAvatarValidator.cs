using FluentValidation;

namespace Services.User.Validators;

public class UserUploadAvatarValidator : AbstractValidator<UploadAvatar.Command>
{
    public UserUploadAvatarValidator()
    {
       RuleFor(x => x.File.Length)
                .NotNull()
                .WithMessage("File is required")
                .NotEmpty()
                .WithMessage("File is required")
                .LessThanOrEqualTo(1 * 1024 * 1024) // 1mb
                .WithMessage("File size is larger than allowed limit 1MB");
        RuleFor(x => x.File.ContentType)
                .Must(x => x.Contains("image"))
                .WithMessage("File must be an image")
                .NotEmpty();
    }
}
