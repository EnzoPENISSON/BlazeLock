using BlazeLock.API.Models;

namespace BlazeLock.API.Repositories
{
    public interface ILogRepository
    {
        Task<HashSet<Log>> GetAllAsync();
        Task<Log?> GetByIdAsync(Guid idLog);
        Task<HashSet<Log>> GetByCoffreAsync(Guid idCoffre);
        Task AddAsync(Log utilisateur);
        Task SaveChangesAsync();
    }
}
