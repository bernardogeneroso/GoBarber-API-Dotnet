using System.Text;
using Application.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Models;
using Services.User.Validation;

namespace Services.User;

public class VerifyEmail
{
    public class Command : IRequest<Result<Unit>>
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x).SetValidator(new UserVerifyEmailValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly UserManager<AppUser> _userManager;
        public Handler(UserManager<AppUser> userManager)
        {
            this._userManager = userManager;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null) return Result<Unit>.Unauthorized();

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(request.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded) return Result<Unit>.Failure("Failed to verify email");

            return Result<Unit>.SuccessNoContent(Unit.Value);
        }
    }
}
