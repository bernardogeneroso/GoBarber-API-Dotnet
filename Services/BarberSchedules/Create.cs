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
        public BarberScheduleDtoRequest BarberSchedule { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.BarberSchedule).SetValidator(new BarberScheduleValidator());
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
            var userId = this._userAccessor.GetIdentity();

            if (userId == null) return Result<Unit>.Failure("Failed to create schedule");

            var scheduleExist = await this._context.BarberSchedules
                    .AnyAsync(x =>
                        x.BarberId == userId &&
                        x.DayOfWeek == request.BarberSchedule.DayOfWeek,
                        cancellationToken
                    );

            if (scheduleExist) return Result<Unit>.Failure("You already have a schedule for this day");

            var barberSchedule = this._mapper.Map<BarberSchedule>(request.BarberSchedule);

            barberSchedule.BarberId = userId;

            this._context.BarberSchedules.Add(barberSchedule);

            var result = await this._context.SaveChangesAsync(cancellationToken) > 0;

            if (!result) return Result<Unit>.Failure("Failed to create barber schedule");

            return Result<Unit>.SuccessNoContent(Unit.Value);
        }
    }
}
