using Application.Core;
using AutoMapper;
using Database;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Appointments.DTOs;
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
        public CommandValidator(DataContext context)
        {
            RuleFor(x => x.Appointment).SetValidator(new AppointmentCreateValidator(context));
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
                    .Select(x => new { x.Email, x.Id, x.IsBarber })
                    .FirstOrDefaultAsync(x => x.Email == this._userAccessor.GetEmail(), cancellationToken);

            if (user is null) return Result<Unit>.Failure("Failed to create appointment");

            if (user.Id == request.Appointment.BarberId) return Result<Unit>.Failure("You can't book an appointment with yourself");

            var appointment = this._mapper.Map<Appointment>(request.Appointment);

            if (await this._context.Appointments
                    .AnyAsync(x =>
                        x.BarberId == request.Appointment.BarberId &&
                        x.Date.Equals(appointment.Date), cancellationToken))
                return Result<Unit>.Failure("This appointment already exists");

            var appointmentCount = await this._context.Appointments
                    .CountAsync(x =>
                        x.CustomerId == user.Id &&
                        x.BarberId == appointment.BarberId &&
                        x.Date.Date == appointment.Date.Date,
                        cancellationToken);

            if (appointmentCount > 0) return Result<Unit>.Failure("You can only create one appointment per day");

            appointment.CustomerId = user.Id;

            this._context.Appointments.Add(appointment);

            var result = await this._context.SaveChangesAsync(cancellationToken) > 0;

            if (!result) return Result<Unit>.Failure("Failed to create appointment");

            return Result<Unit>.SuccessNoContent(Unit.Value);
        }
    }
}
