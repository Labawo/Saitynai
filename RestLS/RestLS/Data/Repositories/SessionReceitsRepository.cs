using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data.Repositories;


public interface ISessionReceitsRepository
{
    Task<SessionReceit?> GetAsync(int doctorId, int groupSessionId, int sessionReceitId);
    Task<IReadOnlyList<SessionReceit>> GetManyAsync(int doctorId, int groupSessionId);
    Task CreateAsync(SessionReceit sessionReceit);
    Task RemoveAsync(SessionReceit sessionReceit);
}

public class SessionReceitsRepository : ISessionReceitsRepository
{
    private readonly LS_DbContext _lsDbContext;

    public SessionReceitsRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }

    public async Task<SessionReceit?> GetAsync(int doctorId, int groupSessionId, int sessionReceitId)
    {
        return await _lsDbContext.SessionReceits.FirstOrDefaultAsync(o => o.Id == sessionReceitId && o.GroupSes.Id == groupSessionId && o.GroupSes.Doc.Id == doctorId);
    }

    public async Task<IReadOnlyList<SessionReceit>> GetManyAsync(int doctorId, int groupSessionId)
    {
        return await _lsDbContext.SessionReceits.Where(o => o.GroupSes.Doc.Id == doctorId && o.GroupSes.Id == groupSessionId).ToListAsync();
    }

    public async Task CreateAsync(SessionReceit sessionReceit)
    {
        _lsDbContext.SessionReceits.Add(sessionReceit);
        await _lsDbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(SessionReceit sessionReceit)
    {
        _lsDbContext.SessionReceits.Remove(sessionReceit);
        await _lsDbContext.SaveChangesAsync();
    }
    
}