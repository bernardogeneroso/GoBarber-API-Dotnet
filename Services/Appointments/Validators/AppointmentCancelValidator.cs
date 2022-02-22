using FluentValidation;
using Services.Appointments.DTOs;
using Services.Appointments.Helpers;

namespace Services.Appointments.Validators;

public class AppointmentCancelValidator : AbstractValidator<AppointmentDtoCancel>
{
    public AppointmentCancelValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required")
            .Must(BeValidDate)
            .WithMessage("Date is invalid, must be rounded to 30 minutes");
    }

    private bool BeValidDate(DateTime date)
    {
        return DateTimeHelper.ValidateDate(date);
    }
}
