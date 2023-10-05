using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data.Repositories;

public interface IAdminsRepository
{
    Task<Admin?> GetAsync(int adminId);
    Task<IReadOnlyList<Admin>> GetManyAsync();
    Task CreateAsync(Admin admin);
    Task UpdateAsync(Admin admin);
    Task RemoveAsync(Admin admin);
}

public class AdminsRepository : IAdminsRepository
{
    private readonly LS_DbContext _lsDbContext;
    
    public AdminsRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }

    public async Task<Admin?> GetAsync(int adminId)
    {
        return await _lsDbContext.Admins.FirstOrDefaultAsync(o => o.Id == adminId);
    }

    public async Task<IReadOnlyList<Admin>> GetManyAsync()
    {
        return await _lsDbContext.Admins.ToListAsync();
    }

    public async Task CreateAsync(Admin admin)
    {
        _lsDbContext.Admins.Add(admin);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Admin admin)
    {
        _lsDbContext.Admins.Update(admin);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(Admin admin)
    {
        _lsDbContext.Admins.Remove(admin);
        await _lsDbContext.SaveChangesAsync();
    }
}