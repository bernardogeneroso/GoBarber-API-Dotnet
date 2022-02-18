using AutoMapper;
using Models;
using Models.Helpers;
using Services.User.DTOs;

namespace Services.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        string currentOrigin = null;

        CreateMap<UserDtoCreateRequest, AppUser>();
        CreateMap<AppUser, UserDtoQuery>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => 
                        src.AvatarName != null ? 
                        new AvatarDto { Name = src.AvatarName, Url = $"{currentOrigin}/{src.AvatarName}" } 
                        : null
                ));
        CreateMap<AppUser, UserDtoSession>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => 
                        src.AvatarName != null ? 
                        new AvatarDto { Name = src.AvatarName, Url = $"{currentOrigin}/{src.AvatarName}" } 
                        : null
                ));
    }
}

