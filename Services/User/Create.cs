using System.Text;
using Application.Core;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Models;
using Services.Interfaces;
using Services.User.DTOs;
using Services.User.Validators;

namespace Services.User;

public class Create
{
    public class Command : IRequest<Result<Unit>>
    {
        public UserDtoCreateRequest User { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator(UserManager<AppUser> userManager)
        {

            RuleFor(x => x.User).SetValidator(new UserCreateValidator(userManager));
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IApiAccessor _apiAccessor;
        private readonly IMailAccessor _mailAccessor;
        public Handler(UserManager<AppUser> userManager, IMapper mapper, IApiAccessor apiAccessor, IMailAccessor mailAccessor)
        {
            this._mailAccessor = mailAccessor;
            this._apiAccessor = apiAccessor;
            this._mapper = mapper;
            this._userManager = userManager;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<AppUser>(request.User);

            var result = await this._userManager.CreateAsync(user, request.User.Password);

            if (!result.Succeeded) return Result<Unit>.Failure("Failed to create user");

            var origin = this._apiAccessor.GetOrigin();

            var token = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var verificationLink = $"{origin}/account/verify-email?token={token}&email={user.Email}";

            var button = new MailButton
            {
                Text = "Verify Email",
                Link = verificationLink
            };

            var body = $"Please confirm your email by clicking on the button below.";

            var resultMail = await this._mailAccessor.SendMail(
                    user.Email, 
                    "GoBarber - Confirm your email", 
                    user.DisplayName, 
                    button, 
                    body
            );

            if (!resultMail) return Result<Unit>.Failure("Failed to send email");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
