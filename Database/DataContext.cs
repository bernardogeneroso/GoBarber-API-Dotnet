using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Database;

public class DataContext : IdentityDbContext<AppUser>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<BarberSchedule> BarberSchedules { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<BarberSchedule>()
            .HasOne(bs => bs.User)
            .WithMany(u => u.BarberSchedules)
            .HasForeignKey(bs => bs.UserId);

        builder.Entity<Appointment>()
            .HasOne(a => a.User)
            .WithMany(u => u.ClientAppointments)
            .HasForeignKey(a => a.UserId);

        builder.Entity<Appointment>()
            .HasOne(a => a.Barber)
            .WithMany(b => b.BarberAppointments)
            .HasForeignKey(a => a.BarberId);
    }

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        AddTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x =>
                    (x.Entity is IdentityUser || x.Entity is BaseEntity) &&
                    (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow; // current datetime

            if (entity.Entity is IdentityUser user)
            {
                if (entity.State == EntityState.Added)
                {
                    ((AppUser)user).CreatedAt = now;
                }
                ((AppUser)user).UpdatedAt = now;
            }
            else
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedAt = now;
                }
                ((BaseEntity)entity.Entity).UpdatedAt = now;
            }
        }
    }
}
