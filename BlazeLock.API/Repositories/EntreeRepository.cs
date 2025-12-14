using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace BlazeLock.API.Repositories
{
    public class EntreeRepository : IEntreeRepository
    {
        private readonly IDbContextFactory<BlazeLockContext> _contextFactory;

        public EntreeRepository(IDbContextFactory<BlazeLockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<HashSet<Entree>> GetAllAsync()
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var coffres = await context.Entrees.AsNoTracking().ToListAsync();
            return coffres.ToHashSet();
        }

        public async Task<Entree?> GetByIdAsync(Guid idEntree)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            return await context.Entrees.FindAsync(idEntree);
        }

        public async Task<HashSet<Entree>> GetAllByDossierAsync(Guid idDossier)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var coffres = await context.Entrees
                .Where(e => e.IdDossier == idDossier)
                .AsNoTracking()
                .ToListAsync();

            return coffres.ToHashSet();
        }

        public async Task AddAsync(Entree coffre)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            await context.Entrees.AddAsync(coffre);
            await context.SaveChangesAsync();
        }

        public async Task DeleteEntree(Entree coffre)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            context.Entrees.Remove(coffre);
            await context.SaveChangesAsync();
        }
    }
}
