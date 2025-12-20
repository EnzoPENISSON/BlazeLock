using BlazeLock.DbLib;

namespace BlazeLock.FRONT.Services
{
    public interface IDossierAPIService
    {
        Task<List<DossierDto>> GetFoldersByCoffreAsync(Guid coffreId);
        Task<DossierDto?> GetDossierByIdAsync(Guid coffreId, Guid dossierId);
        Task<bool> CreateDossierAsync(DossierDto dossier);
        Task<bool> UpdateDossierAsync(DossierDto dossier);
        Task<bool> DeleteDossierAsync(Guid coffreId, Guid dossierId);
    }
}