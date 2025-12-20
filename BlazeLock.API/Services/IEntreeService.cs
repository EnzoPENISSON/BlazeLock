using BlazeLock.DbLib;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Services
{
    public interface IEntreeService
    {
        Task<HashSet<EntreeDto>> GetAllAsync(Guid idCoffre);
        Task<HashSet<EntreeDto>> GetAllByDossierAsync(Guid idCoffre, Guid IdDossier);
        Task<EntreeDto?> GetByIdAsync(Guid idEntree);
        Task<EntreeHistoriqueDto?> GetByIdWithHistoriqueAsync(Guid idCoffre,Guid idEntree);
        Task AddAsync(EntreeDto dto);
        Task updateAsync(Guid idEntree, Guid IdDossier);
        Task<IActionResult?> VerifyUserAccess(EntreeDto entreeDto, (Guid, IActionResult?) utilisateur);
        Task<HashSet<EntreeDto>> GetAllByCoffreAsync(Guid idCoffre);
        Task AddLog(EntreeDto entree, Guid idUtilisateur, string message);
        Task DeleteEntree(Guid id);

    }
}
