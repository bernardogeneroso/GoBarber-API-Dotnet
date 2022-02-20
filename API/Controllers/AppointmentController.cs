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
}
