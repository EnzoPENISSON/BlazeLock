using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace BlazeLock.API.Repositories
{
    public class PartageRepository : IPartageRepository
    {
        private readonly IDbContextFactory<BlazeLockContext> _contextFactory;

        public PartageRepository(IDbContextFactory<BlazeLockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<HashSet<Partage>> GetAllAsync()
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var partages = await context.Partages.AsNoTracking().ToListAsync();
            return partages.ToHashSet();
        }

        public async Task<HashSet<Partage>> GetByCoffreAsync(Guid idCoffre)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var partages = await context.Partages
                .Where(p => p.IdCoffre == idCoffre)
                .AsNoTracking()
                .ToListAsync();

            return partages.ToHashSet();
        }

        public async Task<HashSet<Partage>> GetByUtilisateurAsync(Guid idUtilisateur)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var partages = await context.Partages
                .Where(p => p.IdUtilisateur == idUtilisateur)
                .AsNoTracking()
                .ToListAsync();

            return partages.ToHashSet();
        }

        public async Task AddAsync(Partage partage)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            await context.Partages.AddAsync(partage);
            await context.SaveChangesAsync();
        }


        public async Task DeletePartage(Partage partage)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            context.Partages.Remove(partage);
            await context.SaveChangesAsync();
        }
    }
}
