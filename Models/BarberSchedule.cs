namespace Models;

public class BarberSchedule : BaseEntity
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? StartInterval { get; set; }
    public DateTime? EndInterval { get; set; }
    public bool IsAvailable => StartTime.HasValue && EndTime.HasValue;
    public bool IsInterval
    {
        get
        {
            var dateNow = DateTime.UtcNow;

            return StartInterval.HasValue 
                && EndInterval.HasValue 
                && dateNow >= StartInterval 
                && dateNow <= EndInterval;
        }
    }
    public AppUser User { get; set; }
}
