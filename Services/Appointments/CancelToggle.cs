using Application.Core;
using Database;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Enum;
using Services.Appointments.DTOs;
using Services.Appointments.Validators;
using Services.Interfaces;

namespace Services.Appointments;

public class CancelToggle
{
    public class Command : IRequest<Result<Unit>>
    {
        public AppointmentDtoCancel Appointment { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Appointment).SetValidator(new AppointmentCancelValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
        public Handler(DataContext context, IUserAccessor userAccessor)
        {
            this._userAccessor = userAccessor;
            this._context = context;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await this._context.Users
                    .Select(x => new { x.Email, x.Id, x.IsBarber })
                    .FirstOrDefaultAsync(x => x.Email == this._userAccessor.GetEmail(), cancellationToken);

            if (user is null) return Result<Unit>.Failure("Failed to handle with appointment");

            var appointment = await this._context.Appointments
                    .FirstOrDefaultAsync(x => x.Id == request.Appointment.Id &&
                                            x.Date.Date == request.Appointment.Date.Date,
                                            cancellationToken);

            if (appointment is null) return Result<Unit>.Failure("Failed to handle with appointment");

            if (request.Appointment.BarberId is not null && user.Id != appointment.CustomerId) return Result<Unit>.Failure("You can't cancel this appointment");

            var barberId = appointment.BarberId;
            var isUserABaber = user.Id == barberId;

            if (request.Appointment.BarberId is null && !isUserABaber) return Result<Unit>.Failure("You can't cancel this appointment");

            if ((isUserABaber && appointment.BarberId != user.Id) ||
                (!isUserABaber && appointment.CustomerId != user.Id))
                return Result<Unit>.Failure("You can't cancel this appointment");


            if (
                appointment.IsCancelledBy == Who.Barber && !isUserABaber ||
                appointment.IsCancelledBy == Who.Customer && isUserABaber
                ) return Result<Unit>.Failure("You can't cancel this appointment");

            appointment.IsCancelled = !appointment.IsCancelled;
            appointment.IsCancelledBy = !appointment.IsCancelled ? default : isUserABaber ? Who.Barber : Who.Customer;

            var result = await this._context.SaveChangesAsync(cancellationToken) > 0;

            if (!result) return Result<Unit>.Failure("Failed to handle with the appointment");

            return Result<Unit>.SuccessNoContent(Unit.Value);
        }
    }
}
