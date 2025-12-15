using BlazeLock.API.Models;

namespace BlazeLock.API.Repositories
{
    public interface IUtilisateurRepository
    {
        Task<HashSet<Utilisateur>> GetAllAsync();
        Task<Utilisateur?> GetByIdAsync(Guid id);
        Task AddAsync(Utilisateur utilisateur);
    }
}
