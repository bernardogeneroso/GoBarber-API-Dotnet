using AutoMapper;
using Models;
using Models.Helpers;
using Services.User.DTOs;

namespace Services.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // string currentOrigin = null;

        CreateMap<UserDtoCreateRequest, AppUser>();
        CreateMap<AppUser, UserDtoQuery>();
        CreateMap<AppUser, UserDtoSession>();
    }
}

