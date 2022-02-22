using FluentValidation;
using Services.BarberSchedules.DTOs;

namespace Services.BarberSchedules.Validators;

public class BarberScheduleValidator : AbstractValidator<BarberScheduleDtoRequest>
{
    public BarberScheduleValidator()
    {
        RuleFor(x => x.DayOfWeek)
                .IsInEnum()
                .WithMessage("Day of week is invalid");
        When(x => x.StartHour != null,
                () =>
                {
                    RuleFor(x => x.StartIntervalHour)
                        .NotEmpty()
                        .WithMessage("Start interval hour is required")
                        .GreaterThanOrEqualTo(12)
                        .WithMessage("Start interval hour must be equal or greater than 12")
                        .LessThanOrEqualTo(13)
                        .WithMessage("Start interval hour must be equal less than 13")
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.EndIntervalHour)
                                    .NotEmpty()
                                    .WithMessage("End interval hour is required")
                                    .GreaterThanOrEqualTo(13)
                                    .WithMessage("End interval hour must be greater than 13")
                                    .LessThanOrEqualTo(14)
                                    .WithMessage("End interval hour must be equal or less than 14")
                                    .Must((dto, endIntervalHour) => dto.StartIntervalHour < endIntervalHour)
                                    .WithMessage("End interval hour must be greater than start interval hour");
                        });
                    RuleFor(x => x.StartHour)
                        .NotEmpty()
                        .WithMessage("Start hour is required")
                        .GreaterThanOrEqualTo(8)
                        .WithMessage("Start hour must be equal or greater than 8")
                        .LessThanOrEqualTo(12)
                        .WithMessage("Start hour must be equal less than 12")
                        .Must((dto, startHour) => dto.StartIntervalHour > startHour)
                        .WithMessage("Start hour must be less than start interval hour")
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.EndHour)
                                    .NotEmpty()
                                    .WithMessage("End hour is required")
                                    .GreaterThan(13)
                                    .WithMessage("End hour must be greater than 13")
                                    .LessThanOrEqualTo(20)
                                    .WithMessage("End hour must be equal or less than 20")
                                    .Must((dto, endHour) => dto.StartHour < endHour)
                                    .WithMessage("End hour must be greater than start hour")
                                    .Must((dto, endHour) => dto.EndHour > dto.EndIntervalHour)
                                    .WithMessage("End hour must be greater than end interval hour");
                        });
                }).Otherwise(() =>
                {
                    RuleFor(x => x.EndHour)
                        .Must(IsNull)
                        .WithMessage("End hour must be null");
                    RuleFor(x => x.StartIntervalHour)
                        .Must(IsNull)
                        .WithMessage("Start interval hour must be null");
                    RuleFor(x => x.EndIntervalHour)
                        .Must(IsNull)
                        .WithMessage("End interval hour must be null");
                });
    }

    private bool IsNull(int? value)
    {
        return value == null || value == 0;
    }
}
