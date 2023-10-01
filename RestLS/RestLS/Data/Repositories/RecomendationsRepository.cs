using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data.Repositories;

public interface IRecomendationsRepository
{
    Task<Recomendation?> GetAsync(int doctorId, int appointmentId, int recomendationId);
    Task<IReadOnlyList<Recomendation>> GetManyAsync(int doctorId, int appointmentId);
    Task CreateAsync(Recomendation recomendation);
    Task UpdateAsync(Recomendation recomendation);
    Task RemoveAsync(Recomendation recomendation);
}

public class RecomendationsRepository : IRecomendationsRepository
{
    private readonly LS_DbContext _lsDbContext;

    public RecomendationsRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }

    public async Task<Recomendation?> GetAsync(int doctorId, int appointmentId, int recomendationId)
    {
        return await _lsDbContext.Recomendations.FirstOrDefaultAsync(o => o.ID == recomendationId && o.Appoint.ID == appointmentId && o.Appoint.Doc.Id == doctorId);
    }

    public async Task<IReadOnlyList<Recomendation>> GetManyAsync(int doctorId, int appointmentId)
    {
        return await _lsDbContext.Recomendations.Where(o => o.Appoint.Doc.Id == doctorId && o.Appoint.ID == appointmentId).ToListAsync();
    }

    public async Task CreateAsync(Recomendation recomendation)
    {
        _lsDbContext.Recomendations.Add(recomendation);
        await _lsDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Recomendation recomendation)
    {
        _lsDbContext.Recomendations.Update(recomendation);
        await _lsDbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(Recomendation recomendation)
    {
        _lsDbContext.Recomendations.Remove(recomendation);
        await _lsDbContext.SaveChangesAsync();
    }
    
}