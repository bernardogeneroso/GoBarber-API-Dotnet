using Microsoft.AspNetCore.Mvc;
using Services.BarberSchedules;
using Services.BarberSchedules.DTOs;

namespace API.Controllers;

public class BarberController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create(BarberScheduleDtoCreate schedule)
    {
        return HandleResult(await Mediator.Send(new Create.Command { BarberSchedule = schedule }));
    }
}
