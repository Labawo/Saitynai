using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data;

public class AppointmentDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Recomendation> Recomendations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=AppointmentDB2");
    }
}