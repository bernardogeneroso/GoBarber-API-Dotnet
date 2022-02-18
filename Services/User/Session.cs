using Application.Core;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Models;
using Services.Interfaces;
using Services.User.DTOs;
using Services.User.Validation;

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
        private readonly ITokenAccessor _tokenAccessor;
        private readonly IMapper _mapper;
        public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenAccessor tokenAccessor, IMapper mapper)
        {
            this._mapper = mapper;
            this._tokenAccessor = tokenAccessor;
            this._signInManager = signInManager;
            this._userManager = userManager;
        }

        public async Task<Result<UserDtoSession>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await this._userManager.FindByEmailAsync(request.User.Email);

            if (user is null) return Result<UserDtoSession>.Failure("Invalid email or password");

            var result = await this._signInManager.CheckPasswordSignInAsync(user, request.User.Password, false);

            if (!result.Succeeded) return Result<UserDtoSession>.Failure("Invalid email or password");

            var token = this._tokenAccessor.CreateToken(user);

            var userDtoSession = this._mapper.Map<UserDtoSession>(user);

            userDtoSession.Token = token;
            
            return Result<UserDtoSession>.Success(userDtoSession);
        }
    }
}
