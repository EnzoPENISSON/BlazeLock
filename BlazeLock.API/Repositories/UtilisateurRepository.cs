using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazeLock.API.Repositories
{
    public class UtilisateurRepository : IUtilisateurRepository
    {
        private readonly BlazeLockContext _context;

        public UtilisateurRepository(BlazeLockContext context)
        {
            _context = context;
        }

        public async Task<HashSet<Utilisateur>> GetAllAsync()
        {
            var utilisateurs = await _context.Utilisateurs.ToListAsync();
            return utilisateurs.ToHashSet();
        }

        public async Task<Utilisateur?> GetByIdAsync(Guid id)
        {
            return await _context.Utilisateurs.FindAsync(id);
        }

        public async Task AddAsync(Utilisateur utilisateur)
        {
            await _context.Utilisateurs.AddAsync(utilisateur);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
