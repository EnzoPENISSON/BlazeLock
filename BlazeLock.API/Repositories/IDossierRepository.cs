using BlazeLock.API.Models;
using System.Threading.Tasks;

namespace BlazeLock.API.Repositories
{
    public interface IDossierRepository
    {
        Task<HashSet<Dossier>> GetAllAsync();
        Task<Dossier?> GetByIdAsync(Guid idDossier);
        Task<HashSet<Dossier>> GetByCoffreAsync(Guid idUtilisateur);
        Task AddAsync(Dossier partage);
        Task DeleteDossier(Dossier partage);
        Task<bool> DossierExistsAsync(string nomDossier, Guid idCoffre);
        Task<Dossier?> GetByLibelleAndCoffreIdAsync(string nomDossier, Guid idCoffre);
    }
}
