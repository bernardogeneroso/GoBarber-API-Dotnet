namespace Services.BarberSchedules.DTOs;

public class BarberScheduleDtoRequest
{
    public DayOfWeek DayOfWeek { get; set; }
    public int? StartHour { get; set; }
    public int? EndHour { get; set; }
    public int? StartIntervalHour { get; set; }
    public int? EndIntervalHour { get; set; }
}
