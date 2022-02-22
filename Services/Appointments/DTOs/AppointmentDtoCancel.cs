namespace Services.Appointments.DTOs;

public class AppointmentDtoCancel
{
    public Guid Id { get; set; }
    public string BarberId { get; set; }
    public DateTime Date { get; set; }
}
