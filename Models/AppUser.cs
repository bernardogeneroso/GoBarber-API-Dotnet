using Microsoft.AspNetCore.Identity;

namespace Models;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; }
    public string AvatarName { get; set; }
    public bool IsBarber { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Appointment> ClientAppointments { get; set; } = new List<Appointment>();
    public ICollection<Appointment> BarberAppointments { get; set; } = new List<Appointment>();
    public ICollection<BarberSchedule> BarberSchedules { get; set; } = new List<BarberSchedule>();
}