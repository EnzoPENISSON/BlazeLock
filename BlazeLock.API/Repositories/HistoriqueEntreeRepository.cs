using BlazeLock.API.Models;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace BlazeLock.API.Repositories
{
    public class HistoriqueEntreeRepository : IHistoriqueEntreeRepository
    {
        private readonly IDbContextFactory<BlazeLockContext> _contextFactory;

        public HistoriqueEntreeRepository(IDbContextFactory<BlazeLockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<HashSet<HistoriqueEntree>> GetAllAsync()
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var coffres = await context.HistoriqueEntrees.AsNoTracking().ToListAsync();
            return coffres.ToHashSet();
        }

        public async Task<HistoriqueEntree?> GetByIdAsync(Guid idHistoriqueEntree)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            return await context.HistoriqueEntrees.FindAsync(idHistoriqueEntree);
        }

        public async Task<HashSet<HistoriqueEntree>> GetAllByEntreeAsync(Guid idEntree)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var result = await context.HistoriqueEntrees
                .Where(e => e.IdEntree == idEntree)
                .AsNoTracking()
                .ToListAsync();

            return result.ToHashSet();
        }

        public async Task<HistoriqueEntree?> GetLastByIdAsync(Guid idHistoriqueEntree)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var result = await context.HistoriqueEntrees
                .Where(h => h.IdEntree == idHistoriqueEntree)
                .OrderByDescending(h => h.DateUpdate)
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task AddAsync(HistoriqueEntree he)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            await context.HistoriqueEntrees.AddAsync(he);
            await context.SaveChangesAsync();
        }

        public async Task DeleteHistoriqueEntree(HistoriqueEntree he)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            context.HistoriqueEntrees.Remove(he);
            await context.SaveChangesAsync();
        }
    }
}
