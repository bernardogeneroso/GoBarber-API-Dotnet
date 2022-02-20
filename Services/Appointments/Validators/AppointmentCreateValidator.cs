using FluentValidation;
using Services.Appointments.DTOs;

namespace Services.Appointments.Validators;


// Allow the date to be between 9 AM to 19 PM
// Allow the date to be at least 30 minutes after the current time
// Allow the date 30 minute date skipping
public class AppointmentCreateValidator : AbstractValidator<AppointmentDtoCreate>
{
    public AppointmentCreateValidator()
    {
        RuleFor(x => x.BarberId)
                .NotEmpty()
                .WithMessage("Barber id is required")
                .NotNull()
                .WithMessage("Barber id is required");
        RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Date is required")
                .Must(BeAValidDate)
                .WithMessage("Date must be 1 hour in the future");
    }

    private bool BeAValidDate(DateTime date)
    {
        if (date.Equals(default)) return false;

        return date >= DateTime.Now.AddMinutes(59);
    }
}
