using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NuGet.Protocol.Core.Types;

namespace BlazeLock.API.Repositories
{
    public class CoffreRepository : ICoffreRepository
    {
        private readonly IDbContextFactory<BlazeLockContext> _contextFactory;

        public CoffreRepository(IDbContextFactory<BlazeLockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<HashSet<Coffre>> GetAllAsync()
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var coffres = await context.Coffres.AsNoTracking().ToListAsync();
            return coffres.ToHashSet();
        }

        public async Task<Coffre?> GetByIdAsync(Guid? idCoffre)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Coffres.FindAsync(idCoffre);
        }

        public async Task<HashSet<Coffre>> GetByUtilisateurAsync(Guid idUtilisateur)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var coffres = await context.Coffres
                .Where(p => p.IdUtilisateur == idUtilisateur)
                .AsNoTracking()
                .ToListAsync();

            return coffres.ToHashSet();
        }

        public async Task AddAsync(Coffre coffre)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            await context.Coffres.AddAsync(coffre);
            await context.SaveChangesAsync();
        }

        public async Task DeleteCoffre(Coffre coffre)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            context.Coffres.Remove(coffre);
            await context.SaveChangesAsync();
        }

    }
}
