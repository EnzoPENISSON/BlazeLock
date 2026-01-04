using BlazeLock.DbLib;

namespace BlazeLock.FRONT.Services
{
    public interface IEntreeAPIService
    {
        Task<List<EntreeDto>> GetAllByCoffreAsync(Guid coffreId);
        Task<List<EntreeDto>> GetAllByDossierAsync(Guid idCoffre, Guid dossierId);
        Task<EntreeDto?> GetByIdAsync(Guid idCoffre, Guid id);
        Task<bool> CreateEntreeAsync(EntreeDto entree);
        Task<bool> UpdateDossierAsync(Guid idCoffre, Guid targetFolderId, Guid entryId);
        Task<bool> DeleteEntreeAsync(Guid idCoffre, Guid id);
        Task<EntreeHistoriqueDto?> GetByIdWithHistoriqueAsync(Guid idCoffre, Guid id);
    }
}