using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace BlazeLock.API.Repositories
{
    public class PartageRepository : IPartageRepository
    {
        private readonly BlazeLockContext _context;

        public PartageRepository(BlazeLockContext context)
        {
            _context = context;
        }

        public async Task<HashSet<Partage>> GetAllAsync()
        {
            var partages = await _context.Partages.ToListAsync();
            return partages.ToHashSet();
        }

        public async Task<HashSet<Partage>> GetByCoffreAsync(Guid idCoffre)
        {
            var partages = await _context.Partages
                .Where(p => p.IdCoffre == idCoffre)
                .AsNoTracking()
                .ToListAsync();

            return partages.ToHashSet();
        }

        public async Task<HashSet<Partage>> GetByUtilisateurAsync(Guid idUtilisateur)
        {
            var partages = await _context.Partages
                .Where(p => p.IdUtilisateur == idUtilisateur)
                .AsNoTracking()
                .ToListAsync();

            return partages.ToHashSet();
        }

        public async Task AddAsync(Partage partage)
        {
            await _context.Partages.AddAsync(partage);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task DeletePartage(Partage partage)
        {
            _context.Partages.Remove(partage);
            return Task.CompletedTask;
        }
    }
}
