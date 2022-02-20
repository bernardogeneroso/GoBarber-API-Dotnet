using Application.Core;
using AutoMapper;
using Database;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Appointments.DTOs;
using Services.Appointments.Helpers;
using Services.Appointments.Validators;
using Services.Interfaces;

namespace Services.Appointments;

public class Create
{
    public class Command : IRequest<Result<Unit>>
    {
        public AppointmentDtoCreate Appointment { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Appointment).SetValidator(new AppointmentCreateValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;
        public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper)
        {
            this._userAccessor = userAccessor;
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await this._context.Users
                    .Select(x => new { x.Email, x.Id })
                    .FirstOrDefaultAsync(x => x.Email == this._userAccessor.GetEmail(), cancellationToken);

            if (user is null) return Result<Unit>.Failure("Failed to create appointment");

            var barberUser = await this._context.Users
                    .Select(x => new { x.IsBarber, x.Id })
                    .FirstOrDefaultAsync(x => x.Id == request.Appointment.BarberId, cancellationToken);

            if (barberUser is null || !barberUser.IsBarber) return Result<Unit>.Failure("Failed to create appointment");

            var appointment = this._mapper.Map<Appointment>(request.Appointment);
            
            // TODO: Check if appointment is available, doesn't work for now
            if (await this._context.Appointments.AnyAsync(x => appointment.Date == x.Date, cancellationToken))
                    Result<Unit>.Failure("This appointment already exists");
            
            appointment.UserId = user.Id;

            this._context.Appointments.Add(appointment);

            var result = await this._context.SaveChangesAsync(cancellationToken) > 0;

            if (!result) return Result<Unit>.Failure("Failed to create appointment");

            return Result<Unit>.SuccessNoContent(Unit.Value);
        }
    }
}
