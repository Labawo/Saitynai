using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data;

public class LS_DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    protected readonly IConfiguration Configuration;

    public LS_DbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to mysql with connection string from app settings
        var connectionString = Configuration.GetConnectionString("WebApiDatabase");
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Recomendation> Recomendations { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Patient> Patient { get; set; }
    public DbSet<GroupSession> GroupSessions { get; set; }
    
    public DbSet<SessionReceit> SessionReceits { get; set; }
    //public DbSet<User> UsersRepo { get; set; }
    

    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=AppointmentDB2");
    }*/
    
}