using BlazeLock.DbLib;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Services
{
    public interface IDossierService
    {
        Task<HashSet<DossierDto>> GetAllAsync();
        Task<DossierDto?> GetByIdAsync(Guid id);
        Task<HashSet<DossierDto>> GetByCoffreAsync(Guid id);
        Task AddAsync(DossierDto dto);
        Task Delete(DossierDto dto);
        Task<IActionResult?> VerifyUserAccess(DossierDto dossierDto, (Guid, IActionResult?) utilisateur);
        Task AddLog(DossierDto dossier, Guid idUtilisateur, string message);
    }
}
