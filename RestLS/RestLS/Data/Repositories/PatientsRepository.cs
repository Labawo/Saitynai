using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data.Repositories;

public interface IPatientsRepository
{
    Task<Patient?> GetAsync(int patientId);
    Task<IReadOnlyList<Patient>> GetManyAsync();
    Task CreateAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task RemoveAsync(Patient patient);
}

public class PatientsRepository : IPatientsRepository
{
    private readonly LS_DbContext _lsDbContext;
    
    public PatientsRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }

    public async Task<Patient?> GetAsync(int patientId)
    {
        return await _lsDbContext.Patient.FirstOrDefaultAsync(o => o.Id == patientId);
    }

    public async Task<IReadOnlyList<Patient>> GetManyAsync()
    {
        return await _lsDbContext.Patient.ToListAsync();
    }

    public async Task CreateAsync(Patient patient)
    {
        _lsDbContext.Patient.Add(patient);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Patient patient)
    {
        _lsDbContext.Patient.Update(patient);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(Patient patient)
    {
        _lsDbContext.Patient.Remove(patient);
        await _lsDbContext.SaveChangesAsync();
    }
}