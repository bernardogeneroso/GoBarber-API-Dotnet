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
        string currentOrigin = null;
        DateTime? date = null;

        CreateMap<UserDtoCreateRequest, AppUser>();
        CreateMap<UserDtoRequest, AppUser>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        CreateMap<AppUser, UserDtoQuery>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.AvatarName != null ? new AvatarDto
                {
                    Url = $"{currentOrigin}/images/{src.AvatarName}",
                    Name = src.AvatarName
                } : null));
        CreateMap<AppUser, UserDtoSession>();

        CreateMap<AppointmentDtoCreate, Appointment>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTimeHelper.RoundTo30Minutes(src.Date)));

        CreateMap<BarberScheduleDtoRequest, BarberSchedule>();

        CreateMap<Appointment, AppointmentDtoQuery>()
                .ForMember(dest => dest.IsCancelledBy, opt => opt.MapFrom(src =>
                            src.IsCancelledBy != default ?
                            src.IsCancelledBy.ToString() :
                            null))
                .ForMember(dest => dest.IsCurrentlyActive, opt => opt.MapFrom(src =>
                    date != null &&
                    !src.IsCancelled &&
                    src.Date.Date == date.Value.Date &&
                    date.Value.Hour >= src.Date.Hour &&
                    DateTimeHelper.RoundTo30Minutes(date.Value).Hour == src.Date.Hour &&
                    DateTimeHelper.RoundTo30Minutes(date.Value).Minute == src.Date.Minute
                ));
    }
}