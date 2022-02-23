using Application.Core;
using AutoMapper;
using Database;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.BarberSchedules.DTOs;
using Services.BarberSchedules.Validators;
using Services.Interfaces;

namespace Services.BarberSchedules;

public class Edit
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
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;
        public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
        {
            this._userAccessor = userAccessor;
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = this._userAccessor.GetIdentity();

            if (userId == null) return Result<Unit>.Failure("Failed to edit schedule");

            var schedule = await this._context.BarberSchedules
                .FirstOrDefaultAsync(x =>
                        x.BarberId == userId &&
                        x.DayOfWeek == request.BarberSchedule.DayOfWeek,
                        cancellationToken);

            if (schedule is null) return Result<Unit>.Failure("There is no schedule for this day");

            this._mapper.Map(request.BarberSchedule, schedule);

            var result = await this._context.SaveChangesAsync(cancellationToken) > 0;

            if (!result) return Result<Unit>.Failure("Failed to edit schedule");

            return Result<Unit>.SuccessNoContent(Unit.Value);
        }
    }
}
