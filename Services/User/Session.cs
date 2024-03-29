using Application.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Models;
using Services.User.DTOs;
using Services.User.Utils.Interfaces;
using Services.User.Validators;

namespace Services.User;

public class Session
{
    public class Command : IRequest<Result<UserDtoSession>>
    {
        public UserDtoLoginRequest User { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {

            RuleFor(x => x.User).SetValidator(new UserSessionValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<UserDtoSession>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserMapper _userMapper;
        private readonly IUserRefreshToken _userRefreshToken;
        public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUserMapper userMapper, IUserRefreshToken userRefreshToken)
        {
            this._userRefreshToken = userRefreshToken;
            this._userMapper = userMapper;
            this._signInManager = signInManager;
            this._userManager = userManager;
        }

        public async Task<Result<UserDtoSession>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await this._userManager.FindByEmailAsync(request.User.Email);

            if (user is null) return Result<UserDtoSession>.Unauthorized("Invalid email or password");

            if (!user.EmailConfirmed) return Result<UserDtoSession>.Unauthorized("Email not confirmed");

            var result = await this._signInManager.CheckPasswordSignInAsync(user, request.User.Password, false);

            if (!result.Succeeded) return Result<UserDtoSession>.Unauthorized("Invalid email or password");

            var resultRefreshToken = await this._userRefreshToken.ExecuteAsync(user);

            if (!resultRefreshToken) return Result<UserDtoSession>.Unauthorized("Invalid email or password");
            
            return Result<UserDtoSession>.Success(this._userMapper.ConvertAppUserToUserDtoSession(user));
        }
    }
}
