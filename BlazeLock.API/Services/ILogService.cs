using BlazeLock.DbLib;

namespace BlazeLock.API.Services
{
    public interface ILogService
    {
        Task<HashSet<LogDto>> GetAllAsync();
        Task<HashSet<LogDto>> GetByCoffreAsync(Guid id);
        Task AddAsync(LogDto dto);
    }
}
