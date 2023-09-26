using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data.Repositories;

public interface IDoctorsRepository
{
    Task<Doctor?> GetAsync(int doctorId);
    Task<IReadOnlyList<Doctor>> GetManyAsync();
    Task CreateAsync(Doctor doctor);
    Task UpdateAsync(Doctor doctor);
    Task RemoveAsync(Doctor doctor);
}

public class DoctorsRepository : IDoctorsRepository
{
    private readonly AppointmentDbContext _appointmentDbContext;
    
    public DoctorsRepository(AppointmentDbContext appointmentDbContext)
    {
        _appointmentDbContext = appointmentDbContext;
    }

    public async Task<Doctor?> GetAsync(int doctorId)
    {
        return await _appointmentDbContext.Doctors.FirstOrDefaultAsync(o => o.Id == doctorId);
    }

    public async Task<IReadOnlyList<Doctor>> GetManyAsync()
    {
        return await _appointmentDbContext.Doctors.ToListAsync();
    }

    public async Task CreateAsync(Doctor doctor)
    {
        _appointmentDbContext.Doctors.Add(doctor);
        await _appointmentDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Doctor doctor)
    {
        _appointmentDbContext.Doctors.Update(doctor);
        await _appointmentDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(Doctor doctor)
    {
        _appointmentDbContext.Doctors.Remove(doctor);
        await _appointmentDbContext.SaveChangesAsync();
    }
}