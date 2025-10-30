using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace BlazeLock.API.Repositories
{
    //public class EntreeRepository : IEntreeRepository
    //{
    //    private readonly BlazeLockContext _context;

    //    public EntreeRepository(BlazeLockContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<HashSet<Entree>> GetAllAsync()
    //    {
    //        var coffres = await _context.Entrees.ToListAsync();
    //        return coffres.ToHashSet();
    //    }

    //    public async Task<Entree?> GetByIdAsync(Guid idEntree)
    //    {
    //        return await _context.Entrees.FindAsync(idEntree);
    //    }

    //    public async Task<HashSet<Entree>> GetByDossierAsync(Guid idDossier)
    //    {
    //        //var coffres = await _context.Entrees
    //        //    .Where(e => e.IdDossiers == idDossier)
    //        //    .AsNoTracking()
    //        //    .ToListAsync();

    //        //return coffres.ToHashSet();
    //    }

    //    public async Task AddAsync(Entree coffre)
    //    {
    //        await _context.Entrees.AddAsync(coffre);
    //    }

    //    public async Task SaveChangesAsync()
    //    {
    //        await _context.SaveChangesAsync();
    //    }

    //    public Task DeleteEntree(Entree coffre)
    //    {
    //        _context.Entrees.Remove(coffre);
    //        return Task.CompletedTask;
    //    }
    //}
}
