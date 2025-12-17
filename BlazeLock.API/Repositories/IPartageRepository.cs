using BlazeLock.API.Models;

namespace BlazeLock.API.Repositories
{
    public interface IPartageRepository
    {
        Task<HashSet<Partage>> GetAllAsync();
        Task<HashSet<Partage>> GetByCoffreAsync(Guid idCoffre);
        Task<HashSet<Partage>> GetByUtilisateurAsync(Guid idUtilisateur);
        Task AddAsync(Partage partage);
        Task DeletePartage(Partage partage);
        Task<bool> IsCoffreAdmin(Guid idCoffre, Guid idUtilisateur);
    }
}
