using BlazeLock.API.Models;
using System.Threading.Tasks;

namespace BlazeLock.API.Repositories
{
    public interface ICoffreRepository
    {
        Task<HashSet<Coffre>> GetAllAsync();
        Task<Coffre?> GetByIdAsync(Guid? idCoffre);
        Task<HashSet<Coffre>> GetByUtilisateurAsync(Guid idUtilisateur);
        Task AddAsync(Coffre partage);
        Task Delete(Guid idCoffre);
    }
}
