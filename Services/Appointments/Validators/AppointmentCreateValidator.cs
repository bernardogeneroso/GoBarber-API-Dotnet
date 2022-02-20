using FluentValidation;
using Services.Appointments.DTOs;
using Services.Appointments.Helpers;

namespace Services.Appointments.Validators;

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
                .WithMessage("Date must be between 9 AM and 19PM");
    }

    private bool BeAValidDate(DateTime date)
    {
        if (date.Equals(default)) return false;

        var dateRound = DateTimeHelper.Round(date, TimeSpan.FromMinutes(30));

        if (dateRound.Hour < 9 || dateRound.Hour >= 19) return false;

        return true;
    }
}
