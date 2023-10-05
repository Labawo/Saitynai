using Microsoft.EntityFrameworkCore;
using RestLS.Data.Dtos;
using RestLS.Data.Entities;
using RestLS.Helpers;

/*namespace RestLS.Data.Repositories;

public interface IUsersRepository
{
    Task<User?> GetAsync(int userId);
    Task<IReadOnlyList<User>> GetManyAsync();
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task RemoveAsync(User user);
}

public class UsersRepository : IUsersRepository
{
    private readonly LS_DbContext _lsDbContext;
    
    public UsersRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }

    public async Task<User?> GetAsync(int userId)
    {
        return await _lsDbContext.UsersRepo.FirstOrDefaultAsync(o => o.UserId == userId);
    }

    public async Task<IReadOnlyList<User>> GetManyAsync()
    {
        return await _lsDbContext.UsersRepo.ToListAsync();
    }

    public async Task CreateAsync(User user)
    {
        _lsDbContext.UsersRepo.Add(user);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(User user)
    {
        _lsDbContext.UsersRepo.Update(user);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(User user)
    {
        _lsDbContext.UsersRepo.Remove(user);
        await _lsDbContext.SaveChangesAsync();
    }
}*/