using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Models;
using Services.Interfaces;
using Services.User.DTOs;
using Services.User.Utils.Interfaces;

namespace Services.User;

public class Detail
{
    public class Command : IRequest<Result<UserDtoQuery>>
    {
    }

    public class Handler : IRequestHandler<Command, Result<UserDtoQuery>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserAccessor _userAccessor;
        private readonly IUserMapper _userMapper;
        public Handler(UserManager<AppUser> userManager, IUserAccessor userAccessor, IUserMapper userMapper)
        {
            this._userMapper = userMapper;
            this._userAccessor = userAccessor;
            this._userManager = userManager;
        }

        public async Task<Result<UserDtoQuery>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await this._userManager.FindByEmailAsync(this._userAccessor.GetEmail());

            if (user is null) return Result<UserDtoQuery>.Failure("Failed to get user");

            return Result<UserDtoQuery>.Success(this._userMapper.ConvertAppUserToUserDtoQuery(user));
        }
    }
}
