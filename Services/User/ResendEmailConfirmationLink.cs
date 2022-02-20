using System.Text;
using Application.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Models;
using Services.Interfaces;

namespace Services.User;

public class ResendEmailConfirmationLink
{
    public class Command : IRequest<Result<Unit>>
    {
        public string Email { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email is invalid");
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMailAccessor _mailAccessor;
        private readonly IApiAccessor _ApiAccessor;
        public Handler(UserManager<AppUser> userManager, IMailAccessor mailAccessor, IApiAccessor ApiAccessor)
        {
            this._ApiAccessor = ApiAccessor;
            this._mailAccessor = mailAccessor;
            this._userManager = userManager;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await this._userManager.FindByEmailAsync(request.Email);

            if (user is null) return Result<Unit>.Failure("Failed to resending email confirmation link");

            if (user.EmailConfirmed) return Result<Unit>.Failure("You are already authorized");

            var origin = this._ApiAccessor.GetOrigin();

            var token = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var verificationLink = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";

            var button = new MailButton
            {
                Text = "Verify Email",
                Link = verificationLink
            };

            var body = $"Please verify your email by clicking on the button below.";

            var result = await this._mailAccessor.SendMail(
                    user.Email, 
                    "GoBarber - Verify your email", 
                    user.DisplayName, 
                    button, 
                    body
            );

            if (!result) return Result<Unit>.Failure("Failed to send email");

            return Result<Unit>.SuccessNoContent(Unit.Value);
        }
    }
}
