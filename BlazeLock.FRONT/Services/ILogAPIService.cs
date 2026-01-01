using BlazeLock.DbLib;

namespace BlazeLock.FRONT.Services
{
    public interface ILogAPIService
    {
        Task<PagedResultDto<LogDto>?> GetLogsByVaultIdAsync(Guid vaultId, int pageNumber, int pageSize);
        Task AddLogAsync(Guid vaultId, string message);
    }
}