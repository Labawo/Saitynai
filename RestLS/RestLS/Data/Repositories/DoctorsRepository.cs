using Microsoft.EntityFrameworkCore;
using RestLS.Data.Dtos.Doctors;
using RestLS.Data.Entities;
using RestLS.Helpers;

namespace RestLS.Data.Repositories;

public interface IDoctorsRepository
{
    Task<Doctor?> GetAsync(int doctorId);
    Task<Doctor?> GetAsync(string phone);
    Task<IReadOnlyList<Doctor>> GetManyAsync();
    Task<PagedList<Doctor>> GetManyAsync(DoctorSearchParameters doctorSearchParameters);
    Task CreateAsync(Doctor doctor);
    Task UpdateAsync(Doctor doctor);
    Task RemoveAsync(Doctor doctor);
}

public class DoctorsRepository : IDoctorsRepository
{
    private readonly LS_DbContext _lsDbContext;
    
    public DoctorsRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }

    public async Task<Doctor?> GetAsync(int doctorId)
    {
        return await _lsDbContext.Doctors.FirstOrDefaultAsync(o => o.Id == doctorId);
    }
    
    public async Task<Doctor?> GetAsync(string phone)
    {
        return await _lsDbContext.Doctors.FirstOrDefaultAsync(o => o.PhoneNumb == phone);
    }

    public async Task<IReadOnlyList<Doctor>> GetManyAsync()
    {
        return await _lsDbContext.Doctors.ToListAsync();
    }
    
    public async Task<PagedList <Doctor>> GetManyAsync(DoctorSearchParameters doctorSearchParameters)
    {
        var queryable = _lsDbContext.Doctors.AsQueryable().OrderBy(o => o.Lastname);
        
        return await PagedList<Doctor>.CreateAsync(queryable, doctorSearchParameters.PageNumber, doctorSearchParameters.PageSize);
    }

    public async Task CreateAsync(Doctor doctor)
    {
        _lsDbContext.Doctors.Add(doctor);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Doctor doctor)
    {
        _lsDbContext.Doctors.Update(doctor);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(Doctor doctor)
    {
        _lsDbContext.Doctors.Remove(doctor);
        await _lsDbContext.SaveChangesAsync();
    }
}