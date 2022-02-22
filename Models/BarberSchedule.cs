namespace Models;

public class BarberSchedule : BaseEntity
{
    public Guid Id { get; set; }
    public string BarberId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public int? StartHour { get; set; }
    public int? EndHour { get; set; }
    public int? StartIntervalHour { get; set; }
    public int? EndIntervalHour { get; set; }
    public bool IsAvailable => StartHour.HasValue && EndHour.HasValue;
    public AppUser User { get; set; }
}
