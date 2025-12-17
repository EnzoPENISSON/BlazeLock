using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NuGet.Protocol.Core.Types;

namespace BlazeLock.API.Repositories
{
    public class DossierRepository : IDossierRepository
    {
        private readonly IDbContextFactory<BlazeLockContext> _contextFactory;

        public DossierRepository(IDbContextFactory<BlazeLockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<HashSet<Dossier>> GetAllAsync()
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var dossiers = await context.Dossiers.AsNoTracking().ToListAsync();
            return dossiers.ToHashSet();
        }

        public async Task<Dossier?> GetByIdAsync(Guid idDossier)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Dossiers.FindAsync(idDossier);
        }

        public async Task<HashSet<Dossier>> GetByCoffreAsync(Guid idCoffre)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var dossiers = await context.Dossiers
                .Where(p => p.IdCoffre == idCoffre)
                .AsNoTracking()
                .ToListAsync();

            return dossiers.ToHashSet();
        }

        public async Task AddAsync(Dossier dossier)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            await context.Dossiers.AddAsync(dossier);
            await context.SaveChangesAsync();
        }

        public async Task DeleteDossier(Dossier dossier)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            context.Dossiers.Remove(dossier);
            await context.SaveChangesAsync();
        }

        public async Task<bool> DossierExistsAsync(string nomDossier, Guid idCoffre)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Dossiers
                .AnyAsync(d => d.Libelle == nomDossier && d.IdCoffre == idCoffre);
        }

        public async Task<Dossier?> GetByLibelleAndCoffreIdAsync(string nomDossier, Guid idCoffre)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Dossiers
                .FirstOrDefaultAsync(d => d.Libelle == nomDossier && d.IdCoffre == idCoffre);
        }
    }
}
