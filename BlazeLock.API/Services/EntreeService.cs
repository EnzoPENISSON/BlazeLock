using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazeLock.API.Services
{
    public class EntreeService : IEntreeService
    {
        private readonly BlazeLockContext _db;

        public EntreeService(BlazeLockContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Entree>> GetAllAsync()
        {
            return await _db.Entrees.ToListAsync();
        }

        public async Task<Entree?> GetByIdAsync(Guid id)
        {
            return await _db.Entrees.AsNoTracking()
                .FirstOrDefaultAsync(model => model.IdEntree == id);
        }

        public async Task<Entree> CreateAsync(Entree entree)
        {
            _db.Entrees.Add(entree);
            await _db.SaveChangesAsync();
            return entree;
        }

        public async Task<bool> UpdateAsync(Guid id, Entree entree)
        {
            var affected = await _db.Entrees
                .Where(model => model.IdEntree == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.IdEntree, entree.IdEntree)
                    .SetProperty(m => m.DateCreation, entree.DateCreation)
                );

            return affected == 1;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var affected = await _db.Entrees
                .Where(model => model.IdEntree == id)
                .ExecuteDeleteAsync();

            return affected == 1;
        }
    }
}