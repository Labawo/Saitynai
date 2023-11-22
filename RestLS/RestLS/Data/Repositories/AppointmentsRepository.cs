using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data.Repositories;

public interface IAppointmentsRepository
{
    Task<Appointment?> GetAsync(int therapyId, int appointmentId);
    Task<Appointment?> GetAsync(int therapyId, DateTime date);
    Task<IReadOnlyList<Appointment>> GetManyAsync(int therapyId);
    Task<IReadOnlyList<Appointment>> GetManyAvailableAsync(int therapyId);
    Task<IReadOnlyList<Appointment>> GetManyPatientAsync(string patientId);
    Task CreateAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task RemoveAsync(Appointment appointment);
}

public class AppointmentsRepository : IAppointmentsRepository
{
    private readonly LS_DbContext _lsDbContext;

    public AppointmentsRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }
    
    public async Task<Appointment?> GetAsync(int therapyId, int appointmentId)
    {
        return await _lsDbContext.Appointments.FirstOrDefaultAsync(o => o.ID == appointmentId && o.Therapy.Id == therapyId);
    }
    
    public async Task<Appointment?> GetAsync(int therapyId, DateTime date)
    {
        return await _lsDbContext.Appointments.FirstOrDefaultAsync(o => o.Time <= date && o.Time.AddHours(1) >= date && o.Therapy.Id == therapyId);
    }

    public async Task<IReadOnlyList<Appointment>> GetManyAsync(int therapyId)
    {
        return await _lsDbContext.Appointments.Where(o => o.Therapy.Id == therapyId).ToListAsync();
    }
    
    public async Task<IReadOnlyList<Appointment>> GetManyAvailableAsync(int therapyId)
    {
        return await _lsDbContext.Appointments.Where(o => o.Therapy.Id == therapyId && o.IsAvailable == true && o.Time >= DateTime.UtcNow.AddDays(1)).ToListAsync();
    }
    
    public async Task<IReadOnlyList<Appointment>> GetManyPatientAsync(string patientId)
    {
        return await _lsDbContext.Appointments.Where(o => o.PatientId == patientId).ToListAsync();
    }
    
    public async Task CreateAsync(Appointment appointment)
    {
        _lsDbContext.Appointments.Add(appointment);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Appointment appointment)
    {
        _lsDbContext.Appointments.Update(appointment);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(Appointment appointment)
    {
        _lsDbContext.Appointments.Remove(appointment);
        await _lsDbContext.SaveChangesAsync();
    }
}