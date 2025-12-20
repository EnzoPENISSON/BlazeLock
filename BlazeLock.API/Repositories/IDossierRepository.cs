using BlazeLock.API.Models;
using System.Threading.Tasks;

namespace BlazeLock.API.Repositories
{
    public interface IDossierRepository
    {
        Task<HashSet<Dossier>> GetAllAsync();
        Task<Dossier?> GetByIdAsync(Guid idDossier);
        Task<HashSet<Dossier>> GetByCoffreAsync(Guid idUtilisateur);
        Task AddAsync(Dossier dossier);
        Task UpdateDossierAsync(Dossier dossier);
        Task DeleteDossier(Dossier dossier);
        Task<bool> DossierExistsAsync(string nomDossier, Guid idCoffre);
        Task<Dossier?> GetByLibelleAndCoffreIdAsync(string nomDossier, Guid idCoffre);
    }
}
