using AutoMapper;
using Models;
using Models.Helpers;
using Services.Appointments.DTOs;
using Services.User.DTOs;

namespace Services.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // string currentOrigin = null;

        CreateMap<UserDtoCreateRequest, AppUser>();
        CreateMap<UserDtoRequest, AppUser>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        CreateMap<AppUser, UserDtoQuery>();
        CreateMap<AppUser, UserDtoSession>();

        CreateMap<AppointmentDtoCreate, Appointment>();
    }
}

