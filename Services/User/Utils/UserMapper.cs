using AutoMapper;
using Models;
using Models.Helpers;
using Services.Interfaces;
using Services.User.DTOs;
using Services.User.Utils.Interfaces;

namespace Services.User.Utils;

public class UserMapper : IUserMapper
{
    private readonly IMapper _mapper;
    private readonly IApiAccessor _ApiAccessor;
    public UserMapper(IMapper mapper, IApiAccessor ApiAccessor)
    {
            this._ApiAccessor = ApiAccessor;
            this._mapper = mapper;
    }

    public UserDtoQuery ConvertAppUserToUserDtoQuery(AppUser user)
    {
        return _mapper.Map<AppUser, UserDtoQuery>(user, opt => {
                opt.AfterMap((src, dest) => {
                    dest.Avatar = src.AvatarName is not null ? new AvatarDto {
                        Url = $"{_ApiAccessor.GetOrigin()}/images/{src.AvatarName}",
                        Name = src.AvatarName
                    } : null;
                });
            });;
    }

    public UserDtoSession ConvertAppUserToUserDtoSession(AppUser user)
    {
        return _mapper.Map<AppUser, UserDtoSession>(user, opt => {
                opt.AfterMap((src, dest) => {
                    dest.Avatar = src.AvatarName != null ? new AvatarDto {
                        Url = $"{_ApiAccessor.GetOrigin()}/images/{src.AvatarName}",
                        Name = src.AvatarName
                    } : null;
                });
            });;
    }
}
