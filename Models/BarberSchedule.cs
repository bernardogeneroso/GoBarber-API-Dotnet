namespace Models;

public class BarberSchedule : BaseEntity
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Weekdays Weekday { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? IntervalTime { get; set; }
    public bool IsAvailable => !StartTime.HasValue || !EndTime.HasValue;
    public AppUser User { get; set; }
}
