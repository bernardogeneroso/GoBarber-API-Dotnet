using AutoMapper;
using Models;
using Models.Helpers;
using Services.Appointments.DTOs;
using Services.Appointments.Helpers;
using Services.BarberSchedules.DTOs;
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

        CreateMap<AppointmentDtoCreate, Appointment>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTimeHelper.Round(src.Date, TimeSpan.FromMinutes(30))));
        
        CreateMap<BarberScheduleDtoCreate, BarberSchedule>();
    }
}

