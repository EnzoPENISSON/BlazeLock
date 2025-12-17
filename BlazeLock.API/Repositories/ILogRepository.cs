using BlazeLock.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazeLock.API.Repositories
{
    public interface ILogRepository
    {
        Task<HashSet<Log>> GetAllAsync();
        Task<Log?> GetByIdAsync(Guid idLog);
        Task<HashSet<Log>> GetByCoffreAsync(Guid idCoffre);
        Task<(HashSet<Log>,int)> GetByCoffrePagedAsync(Guid idCoffre, int pageNumber, int pageSize);
        Task AddAsync(Log utilisateur);
    }
}
