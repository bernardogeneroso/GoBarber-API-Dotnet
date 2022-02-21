namespace Services.BarberSchedules.DTOs;

public class BarberScheduleDtoCreate
{
    public DayOfWeek DayOfWeek { get; set; }
    public int? StartHour { get; set; }
    public int? EndHour { get; set; }
    public int? StartIntervalHour { get; set; }
    public int? EndIntervalHour { get; set; }
}
