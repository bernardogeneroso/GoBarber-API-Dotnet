using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.Appointments.DTOs;
using Services.Interfaces;

namespace Services.Appointments;

public class MyAppointments
{
    public class Query : IRequest<Result<List<AppointmentDtoQuery>>>
    {
        public AppointmentDtoDateRequest Appointment { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<List<AppointmentDtoQuery>>>
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;
        private readonly IApiAccessor _apiAccessor;
        public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper, IApiAccessor apiAccessor)
        {
            this._apiAccessor = apiAccessor;
            this._mapper = mapper;
            this._userAccessor = userAccessor;
            this._context = context;
        }

        public async Task<Result<List<AppointmentDtoQuery>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await this._context.Users
                .Select(x => new { x.Email, x.Id, x.IsBarber })
                .FirstOrDefaultAsync(x => x.Email == this._userAccessor.GetEmail(), cancellationToken);

            if (user is null) return Result<List<AppointmentDtoQuery>>.Failure("Failed to query today appointments");

            if (!user.IsBarber) return Result<List<AppointmentDtoQuery>>.Failure("You are not a barber");

            var date = request.Appointment.Date ?? DateTime.UtcNow;

            var appointments = await this._context.Appointments
                .Include(x => x.Customer)
                .Where(x => x.BarberId == user.Id &&
                            x.Date.Date == date.Date
                )
                .OrderBy(x => x.Date)
                .ProjectTo<AppointmentDtoQuery>(this._mapper.ConfigurationProvider, new { currentOrigin = this._apiAccessor.GetOrigin(), date = DateTime.UtcNow })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result<List<AppointmentDtoQuery>>.Success(appointments);
        }
    }
}
