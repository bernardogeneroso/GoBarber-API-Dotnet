using Models;

namespace Services.User.Utils.Interfaces;

public interface IUserRefreshToken
{
    Task<bool> ExecuteAsync(AppUser user);
}
