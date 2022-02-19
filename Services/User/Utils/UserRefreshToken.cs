using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Interfaces;
using Services.User.Utils.Interfaces;

namespace Services.User.Utils;

public class UserRefreshToken : IUserRefreshToken
{
    private readonly IApiAccessor _apiAccessor;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly DataContext _context;
    public UserRefreshToken(DataContext context, IApiAccessor apiAccessor, ITokenAccessor tokenAccessor)
    {
            this._context = context;
            this._tokenAccessor = tokenAccessor;
            this._apiAccessor = apiAccessor;
    }

    public async Task<bool> ExecuteAsync(AppUser user)
    {
        var refreshToken = this._tokenAccessor.GenerateRefreshToken(user.Id);

        var oldRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.User == user);

        if (oldRefreshToken is not null) _context.RefreshTokens.Remove(oldRefreshToken);

        _context.RefreshTokens.Add(refreshToken);

        var result = await _context.SaveChangesAsync() > 0;

        if (!result) return false;

        this._apiAccessor.SetCookie(
                "refreshToken", 
                refreshToken.Token, 
                new CookieOptions {
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddDays(7) 
                }
        );

        return true;
    }
}
