using BlazeLock.DbLib;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Services
{
    public interface IEntreeService
    {
        Task<HashSet<EntreeDto>> GetAllAsync();
        Task<HashSet<EntreeDto>> GetAllByDossierAsync(Guid IdDossier);
        Task<EntreeDto?> GetByIdAsync(Guid idEntree);
        Task<EntreeHistoriqueDto?> GetByIdWithHistoriaqueAsync(Guid idEntree);
        Task AddAsync(EntreeDto dto);
        Task<IActionResult?> VerifyUserAccess(EntreeDto entreeDto, (Guid, IActionResult?) utilisateur);
        Task<HashSet<EntreeDto>> GetAllByCoffreAsync(Guid idCoffre);
        Task AddLog(EntreeDto entree, Guid idUtilisateur, string message);
        //Task Delete(EntreeDto dto);

    }
}
