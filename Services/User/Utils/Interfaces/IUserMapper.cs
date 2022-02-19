using Models;
using Services.User.DTOs;

namespace Services.User.Utils.Interfaces;

public interface IUserMapper
{
    UserDtoQuery ConvertAppUserToUserDtoQuery(AppUser user);
    UserDtoSession ConvertAppUserToUserDtoSession(AppUser user);
}
