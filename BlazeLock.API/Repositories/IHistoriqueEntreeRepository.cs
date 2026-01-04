using BlazeLock.API.Models;
using System.Threading.Tasks;

namespace BlazeLock.API.Repositories
{
    public interface IHistoriqueEntreeRepository
    {
        Task<HashSet<HistoriqueEntree>> GetAllAsync();
        Task<HistoriqueEntree?> GetByIdAsync(Guid idHistoriqueEntree);
        Task<HistoriqueEntree?> GetLastByIdAsync(Guid idHistoriqueEntree);
        Task<HashSet<HistoriqueEntree>> GetAllByEntreeAsync(Guid idEntree);
        Task AddAsync(HistoriqueEntree partage);
        Task DeleteHistoriqueEntree(HistoriqueEntree partage);
    }
}
