using Models;

namespace Services.Interfaces;

public interface ITokenAccessor
{
    string CreateToken(AppUser user);
    RefreshToken GenerateRefreshToken(string userId);
}
