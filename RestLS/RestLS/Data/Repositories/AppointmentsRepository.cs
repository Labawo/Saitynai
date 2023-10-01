using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data.Repositories;

public interface IAppointmentsRepository
{
    Task<Appointment?> GetAsync(int doctorId, int appointmentId);
    Task<IReadOnlyList<Appointment>> GetManyAsync(int doctorId);
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
    
    public async Task<Appointment?> GetAsync(int doctorId, int appointmentId)
    {
        return await _lsDbContext.Appointments.FirstOrDefaultAsync(o => o.ID == appointmentId && o.Doc.Id == doctorId);
    }

    public async Task<IReadOnlyList<Appointment>> GetManyAsync(int doctorId)
    {
        return await _lsDbContext.Appointments.Where(o => o.Doc.Id == doctorId).ToListAsync();
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