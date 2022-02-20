using Application.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Models;
using Services.Interfaces;
using Services.User.Validators;

namespace Services.User;

public class UploadAvatar
{
    public class Command : IRequest<Result<Unit>>
    {
        public IFormFile File { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x).SetValidator(new UserUploadAvatarValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly IImageAccessor _imageAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserAccessor _userAccessor;
        public Handler(IImageAccessor imageAccessor, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            this._userAccessor = userAccessor;
            this._userManager = userManager;
            this._imageAccessor = imageAccessor;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(_userAccessor.GetEmail());

            if (user is null) return Result<Unit>.Failure("Failed to upload image");

            var fileName = await this._imageAccessor.AddImageAsync(request.File);

            if (fileName is null) return Result<Unit>.Failure("Failed to upload image");

            if (user.AvatarName is not null)
            {
                this._imageAccessor.DeleteImage(user.AvatarName);
            }

            user.AvatarName = fileName;
            
            var result = await this._userManager.UpdateAsync(user);

            if (!result.Succeeded) return Result<Unit>.Failure("Failed to upload image");
            
            return Result<Unit>.SuccessNoContent(Unit.Value);
        }
    }
}
