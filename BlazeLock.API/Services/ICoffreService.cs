using BlazeLock.DbLib;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Services
{
    public interface ICoffreService
    {
        Task<HashSet<CoffreDto>> GetAllAsync();
        Task<CoffreDto?> GetByIdAsync(Guid id);
        Task<HashSet<CoffreDto>> GetByUtilisateurAsync(Guid id);
        Task AddAsync(CoffreDto dto);
        Task Delete(Guid id);
        Task<IActionResult?> VerifyUserAccess(CoffreDto coffreDto, (Guid, IActionResult?) utilisateur);
        Task AddLog(Guid? idCoffre, Guid idUtilisateur, string message);
    }
}
