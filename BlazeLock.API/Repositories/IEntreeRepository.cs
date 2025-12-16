using BlazeLock.API.Models;
using System.Threading.Tasks;

namespace BlazeLock.API.Repositories
{
    public interface IEntreeRepository
    {
        Task<HashSet<Entree>> GetAllAsync();
        Task<Entree?> GetByIdAsync(Guid idEntree);
        Task<HashSet<Entree>> GetAllByDossierAsync(Guid idUtilisateur);
        Task<HashSet<Entree>> GetAllByCoffreAsync(Guid idCoffre);
        Task AddAsync(Entree partage);
        Task DeleteEntree(Entree partage);
    }
}
