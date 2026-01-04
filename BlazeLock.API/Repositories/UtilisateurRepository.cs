using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazeLock.API.Repositories
{
    public class UtilisateurRepository : IUtilisateurRepository
    {
        private readonly IDbContextFactory<BlazeLockContext> _contextFactory;

        public UtilisateurRepository(IDbContextFactory<BlazeLockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<HashSet<Utilisateur>> GetAllAsync()
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var utilisateurs = await context.Utilisateurs.AsNoTracking().ToListAsync();
            return utilisateurs.ToHashSet();
        }

        public async Task<Utilisateur?> GetByIdAsync(Guid id)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            return await context.Utilisateurs.FindAsync(id);
        }

        public async Task AddAsync(Utilisateur utilisateur)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            await context.Utilisateurs.AddAsync(utilisateur);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Utilisateur>> SearchByEmailAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<Utilisateur>();

            var context = await _contextFactory.CreateDbContextAsync();

            return await context.Utilisateurs
                .Where(u => u.email.Contains(term))
                .ToListAsync();
        }
    }
}
