using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data.Repositories;


public interface IGroupSessionsRepository
{
    Task<GroupSession?> GetAsync(int doctorId, int groupSessionId);
    Task<GroupSession?> GetAsync(int doctorId, DateTime time);
    Task<IReadOnlyList<GroupSession>> GetManyAsync(int doctorId);
    Task CreateAsync(GroupSession groupSession);
    Task UpdateAsync(GroupSession groupSession);
    Task RemoveAsync(GroupSession groupSession);
}

public class GroupSessionsRepository : IGroupSessionsRepository
{
    private readonly LS_DbContext _lsDbContext;

    public GroupSessionsRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }
    
    public async Task<GroupSession?> GetAsync(int doctorId, int groupSessionId)
    {
        return await _lsDbContext.GroupSessions.FirstOrDefaultAsync(o => o.Id == groupSessionId && o.Doc.Id == doctorId);
    }
    
    public async Task<GroupSession?> GetAsync(int doctorId, DateTime date)
    {
        return await _lsDbContext.GroupSessions.FirstOrDefaultAsync(o => o.Time <= date && o.Time.AddHours(1) >= date && o.Doc.Id == doctorId);
    }

    public async Task<IReadOnlyList<GroupSession>> GetManyAsync(int doctorId)
    {
        return await _lsDbContext.GroupSessions.Where(o => o.Doc.Id == doctorId).ToListAsync();
    }
    
    public async Task CreateAsync(GroupSession groupSession)
    {
        _lsDbContext.GroupSessions.Add(groupSession);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(GroupSession groupSession)
    {
        _lsDbContext.GroupSessions.Update(groupSession);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(GroupSession groupSession)
    {
        _lsDbContext.GroupSessions.Remove(groupSession);
        await _lsDbContext.SaveChangesAsync();
    }
}