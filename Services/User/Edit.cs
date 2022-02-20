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
using Services.User.Utils.Interfaces;
using Services.User.Validators;

namespace Services.User;

public class Edit
{
    public class Command : IRequest<Result<UserDtoSession>>
    {
        public UserDtoRequest User { get; set;}
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.User).SetValidator(new UserValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<UserDtoSession>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserMapper _userMapper;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;
        private readonly IApiAccessor _apiAccessor;
        private readonly IMailAccessor _mailAccessor;
        public Handler(UserManager<AppUser> userManager, IMapper mapper, IUserMapper userMapper, IUserAccessor userAccessor, IApiAccessor apiAccessor, IMailAccessor mailAccessor)
        {
            this._mailAccessor = mailAccessor;
            this._apiAccessor = apiAccessor;
            this._userAccessor = userAccessor;
            this._mapper = mapper;
            this._userMapper = userMapper;
            this._userManager = userManager;
        }

        public async Task<Result<UserDtoSession>> Handle(Command request, CancellationToken cancellationToken)
        {
            var userAccessorEmail = this._userAccessor.GetEmail();

            var user = await this._userManager.FindByEmailAsync(userAccessorEmail);

            if (user is null) return Result<UserDtoSession>.Failure("Failed to update user");

            var checkPassword = await this._userManager.CheckPasswordAsync(user, request.User.LatestPassword);

            if (!checkPassword) return Result<UserDtoSession>.Failure("Wrong password");

            if (request.User.Password is not null)
            {
                var resultChangingPassword = await this._userManager.ChangePasswordAsync(user, request.User.LatestPassword, request.User.Password);

                if (!resultChangingPassword.Succeeded) return Result<UserDtoSession>.Failure("Password doesn't respect the rules");
            }

            if (userAccessorEmail != request.User.Email) user.EmailConfirmed = false;

            _mapper.Map(request.User, user);

            var result = await this._userManager.UpdateAsync(user);

            if (!result.Succeeded) return Result<UserDtoSession>.Failure("Failed to update user");

            if (userAccessorEmail != request.User.Email)
            {
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

                if (!resultMail) return Result<UserDtoSession>.Failure("Failed to send confirmation email");
            }

            return Result<UserDtoSession>.Success(this._userMapper.ConvertAppUserToUserDtoSession(user));
        }
    }
}
