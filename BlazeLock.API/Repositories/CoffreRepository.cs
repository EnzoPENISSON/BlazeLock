using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace BlazeLock.API.Repositories
{
    public class CoffreRepository : ICoffreRepository
    {
        private readonly BlazeLockContext _context;

        public CoffreRepository(BlazeLockContext context)
        {
            _context = context;
        }

        public async Task<HashSet<Coffre>> GetAllAsync()
        {
            var coffres = await _context.Coffres.ToListAsync();
            return coffres.ToHashSet();
        }

        public async Task<Coffre?> GetByIdAsync(Guid idCoffre)
        {
            return await _context.Coffres.FindAsync(idCoffre);
        }

        public async Task<HashSet<Coffre>> GetByUtilisateurAsync(Guid idUtilisateur)
        {
            var coffres = await _context.Coffres
                .Where(p => p.IdUtilisateur == idUtilisateur)
                .AsNoTracking()
                .ToListAsync();

            return coffres.ToHashSet();
        }

        public async Task AddAsync(Coffre coffre)
        {
            await _context.Coffres.AddAsync(coffre);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task DeleteCoffre(Coffre coffre)
        {
            _context.Coffres.Remove(coffre);
            return Task.CompletedTask;
        }
    }
}
