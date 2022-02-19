using Application.Core;
using Database;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.User.DTOs;
using Services.User.Utils.Interfaces;

namespace Services.User;

public class RefreshToken
{
    public class Command : IRequest<Result<UserDtoSession>>
    {
        public string RefreshToken { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                    .NotNull()
                    .WithMessage("Refresh token is required")
                    .NotEmpty()
                    .WithMessage("Refresh token is required");
        }
    }

    public class Handler : IRequestHandler<Command, Result<UserDtoSession>>
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly IUserMapper _userMapper;
        public Handler(DataContext context, IUserAccessor userAccessor, IUserMapper userMapper)
        {
            this._userMapper = userMapper;
            this._userAccessor = userAccessor;
            this._context = context;
        }

        public async Task<Result<UserDtoSession>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == this._userAccessor.GetEmail(), cancellationToken);

            if (user is null) return Result<UserDtoSession>.Unauthorized();

            var refreshToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(x => 
                    x.Token == request.RefreshToken && 
                    x.UserId == user.Id, cancellationToken
            );

            if (refreshToken is null) return Result<UserDtoSession>.Unauthorized();

            if (refreshToken != null && !refreshToken.IsActive) return Result<UserDtoSession>.Unauthorized();

            return Result<UserDtoSession>.Success(this._userMapper.ConvertAppUserToUserDtoSession(user));
        }
    }
}
