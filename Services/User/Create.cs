using Application.Core;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Models;
using Services.User.DTOs;
using Services.User.Validation;

namespace Services.User;

public class Create
{
    public class Command : IRequest<Result<UserDtoQuery>>
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

    public class Handler : IRequestHandler<Command, Result<UserDtoQuery>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public Handler(UserManager<AppUser> userManager, IMapper mapper)
        {
            this._mapper = mapper;
            this._userManager = userManager;
        }

        public async Task<Result<UserDtoQuery>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<AppUser>(request.User);

            var result = await _userManager.CreateAsync(user, request.User.Password);

            if (!result.Succeeded) return Result<UserDtoQuery>.Failure("Failed to create user");

            // TODO: Validate account with email confirmation

            return Result<UserDtoQuery>.Success(_mapper.Map<UserDtoQuery>(user));
        }
    }
}
