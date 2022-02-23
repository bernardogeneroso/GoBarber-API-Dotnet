using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Appointments.DTOs;
using Services.BarberSchedules;
using Services.BarberSchedules.DTOs;

namespace API.Controllers;

public class BarberController : BaseApiController
{
    [Authorize(Policy = "IsBarber")]
    [HttpPost("schedule")]
    public async Task<IActionResult> Create([FromBody] BarberScheduleDtoRequest schedule)
    {
        return HandleResult(await Mediator.Send(new Create.Command { BarberSchedule = schedule }));
    }

    [Authorize(Policy = "IsBarber")]
    [HttpPut("schedule")]
    public async Task<IActionResult> Edit([FromBody] BarberScheduleDtoRequest schedule)
    {
        return HandleResult(await Mediator.Send(new Edit.Command { BarberSchedule = schedule }));
    }

    [HttpPost("appointment/{barberId}")]
    public async Task<IActionResult> CreateAppointment([FromRoute] string barberId, [FromQuery] DateTime date)
    {
        return HandleResult(await Mediator.Send(new Services.Appointments.Create.Command { Appointment = new AppointmentDtoCreate { BarberId = barberId, Date = date } }));
    }

    [HttpPatch("appointment/{id}/cancel")]
    public async Task<IActionResult> CancelAppointment([FromRoute] Guid id, [FromQuery] string barberId, [FromBody] AppointmentDtoCancel appointment)
    {
        appointment.Id = id;
        appointment.BarberId = barberId;

        return HandleResult(await Mediator.Send(new Services.Appointments.CancelToggle.Command { Appointment = appointment }));
    }

    [Authorize(Policy = "IsBarber")]
    [HttpGet("appointment/my-appointments")]
    public async Task<IActionResult> GetMyAppointments([FromQuery] AppointmentDtoDateRequest appointment)
    {
        return HandleResult(await Mediator.Send(new Services.Appointments.MyAppointments.Query { Appointment = appointment }));
    }
}
