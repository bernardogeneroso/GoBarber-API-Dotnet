using Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Appointments.DTOs;
using Services.Appointments.Helpers;

namespace Services.Appointments.Validators;

public class AppointmentCreateValidator : AbstractValidator<AppointmentDtoCreate>
{
        private readonly DataContext _context;
    public AppointmentCreateValidator(DataContext context)
    {
        this._context = context;

        RuleFor(x => x.BarberId)
                .NotEmpty()
                .WithMessage("Barber id is required")
                .NotNull()
                .WithMessage("Barber id is required")
                .MustAsync(async (barberId, cancellation) => await ValidateBarberId(barberId, cancellation))
                .WithMessage("Barber id is invalid");
        RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Date is required")
                .Custom((date, context) => {
                    if (date.Equals(default)) {
                        context.AddFailure("Date is invalid");
                        return;
                    };
                    
                    if (date != DateTimeHelper.Round(date, TimeSpan.FromMinutes(30))) 
                    {
                        context.AddFailure("Date must be rounded to 30 minutes");
                        return;
                    };

                    var barberSchedule = _context.BarberSchedules
                                            .FirstOrDefault(x => x.UserId == context.InstanceToValidate.BarberId
                                                            && (int)x.DayOfWeek == (int)date.DayOfWeek);

                    if (barberSchedule is null 
                        || !barberSchedule.IsAvailable 
                        || barberSchedule.IsInterval)
                    {
                        context.AddFailure("Barber is not available on this period");
                        return;
                    };
                    
                    if (date <= DateTimeHelper.Round(
                                    DateTime.UtcNow.AddMinutes(30), 
                                    TimeSpan.FromMinutes(30)
                                )
                        ) {
                        context.AddFailure("Date must be in next 30 minutes");
                        return;
                    };
                });
    }
    private async Task<bool> ValidateBarberId(string barberId, CancellationToken cancellationToken)
    {
        return await this._context.Users.AnyAsync(x => x.Id == barberId, cancellationToken);
    }
}
