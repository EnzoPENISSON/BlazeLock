using BlazeLock.DbLib;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Services
{
    public interface ILogService
    {
        Task<HashSet<LogDto>> GetAllAsync();
        Task<HashSet<LogDto>> GetByCoffreAsync(Guid id);
        Task<PagedResultDto<LogDto>> GetByCoffrePagedAsync(Guid id, int pageNumber, int pageSize);
        Task Add(Guid idCoffre, Guid idUtilisateur, string message);
        Task<IActionResult?> VerifyUserAccess(Guid idCoffre, (Guid, IActionResult?) utilisateur);
    }
}
