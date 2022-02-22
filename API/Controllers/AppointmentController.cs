using Microsoft.AspNetCore.Mvc;
using Services.Appointments;
using Services.Appointments.DTOs;

namespace API.Controllers;

public class AppointmentController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentDtoCreate appointment)
    {
        return HandleResult(await Mediator.Send(new Create.Command { Appointment = appointment }));
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> CancelAppointment([FromBody] AppointmentDtoCancel appointment)
    {
        return HandleResult(await Mediator.Send(new Cancel.Command { Appointment = appointment }));
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodayAppointments()
    {
        return HandleResult(await Mediator.Send(new TodayAppointments.Command()));
    }
}
