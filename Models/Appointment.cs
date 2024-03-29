using Models.Enum;

namespace Models;

public class Appointment : BaseEntity
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public string BarberId { get; set; }
    public DateTime Date { get; set; }
    public bool IsCancelled { get; set; }
    public Who IsCancelledBy { get; set; }
    public AppUser Customer { get; set; }
    public AppUser Barber { get; set; }
}
