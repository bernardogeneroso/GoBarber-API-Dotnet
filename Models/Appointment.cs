namespace Models;

public class Appointment : BaseEntity
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string BarberId { get; set; }
    public DateTime Date { get; set; }
    public bool IsCancelled { get; set; }
    public Guid IsCancelledBy { get; set; }
    public AppUser User { get; set; }
    public AppUser Barber { get; set; }
}
