using BlazeLock.DbLib;

namespace BlazeLock.FRONT.Services
{
    public interface ILogAPIService
    {
        Task<List<LogDto>?> GetLogsByVaultIdAsync(Guid vaultId);
    }
}
