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

public class Cancel
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

            if (user is null) return Result<Unit>.Failure("Failed to cancel appointment");

            if (!user.IsBarber)
            {
                var barberExist = await this._context.Users
                    .AnyAsync(x => x.Id == request.Appointment.BarberId, cancellationToken);

                if (!barberExist) return Result<Unit>.Failure("Failed to cancel appointment");
            }

            var appointment = await this._context.Appointments
                    .FirstOrDefaultAsync(x =>   x.UserId == user.Id &&
                                                x.BarberId == request.Appointment.BarberId.ToString() &&
                                                x.Date == request.Appointment.Date, cancellationToken);

            if (appointment is null) return Result<Unit>.Failure("Failed to cancel appointment");

            if (appointment.IsCancelled) return Result<Unit>.Failure("Appointment is already cancelled");

            appointment.IsCancelled = true;
            appointment.IsCancelledBy = user.IsBarber ? Who.Barber : Who.Client;

            var result = await this._context.SaveChangesAsync(cancellationToken) > 0;

            if (!result) Result<Unit>.Failure("Failed to cancel appointment");

            return Result<Unit>.SuccessNoContent(Unit.Value);
        }
    }
}
