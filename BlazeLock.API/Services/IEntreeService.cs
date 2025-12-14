using BlazeLock.API.Models;

namespace BlazeLock.API.Services
{
    public interface IEntreeService
    {
        Task<IEnumerable<Entree>> GetAllAsync();
        Task<Entree?> GetByIdAsync(Guid id);
        Task<Entree> CreateAsync(Entree entree);
        Task<bool> UpdateAsync(Guid id, Entree entree);
        Task<bool> DeleteAsync(Guid id);
    }
}
