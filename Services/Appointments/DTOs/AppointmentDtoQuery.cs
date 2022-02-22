using Services.User.DTOs;

namespace Services.Appointments.DTOs;

public class AppointmentDtoQuery
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public string BarberId { get; set; }
    public DateTime Date { get; set; }
    public bool IsCurrentlyActive { get; set; }
    public bool IsCancelled { get; set; }
    public string IsCancelledBy { get; set; }
    public UserDtoQuery Customer { get; set; }
}
