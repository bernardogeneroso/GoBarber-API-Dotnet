using Application.Core;
using AutoMapper;
using Database;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.BarberSchedules.DTOs;
using Services.BarberSchedules.Validators;
using Services.Interfaces;

namespace Services.BarberSchedules;

public class Create
{
    public class Command : IRequest<Result<Unit>>
    {
        public BarberScheduleDtoCreate BarberSchedule { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.BarberSchedule).SetValidator(new BarberScheduleCreateValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
        public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
        {
            this._userAccessor = userAccessor;
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await this._context.Users
                    .Select(x => new { x.Id, x.Email, x.IsBarber })
                    .FirstOrDefaultAsync(x => x.Email == this._userAccessor.GetEmail(), cancellationToken);

            if (user is null) return Result<Unit>.Failure("Failed to create barber schedule");

            if (!user.IsBarber) return Result<Unit>.Failure("You are not a barber");

            var scheduleExist = await this._context.BarberSchedules
                    .AnyAsync(x =>
                        x.BarberId == user.Id &&
                        x.DayOfWeek == request.BarberSchedule.DayOfWeek,
                        cancellationToken
                    );

            if (scheduleExist) return Result<Unit>.Failure("You already have a schedule for this day");

            var barberSchedule = this._mapper.Map<BarberSchedule>(request.BarberSchedule);

            barberSchedule.BarberId = user.Id;

            this._context.BarberSchedules.Add(barberSchedule);

            var result = await this._context.SaveChangesAsync(cancellationToken) > 0;

            if (!result) return Result<Unit>.Failure("Failed to create barber schedule");

            return Result<Unit>.SuccessNoContent(Unit.Value);
        }
    }
}
