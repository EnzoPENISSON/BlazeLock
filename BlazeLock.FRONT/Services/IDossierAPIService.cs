using BlazeLock.DbLib;
using System.Net.Http.Json;

namespace BlazeLock.FRONT.Services
{
    public interface IDossierAPIService
    {
        Task<List<DossierDto>> GetFoldersByCoffreAsync(Guid coffreId);
        Task<bool> CreateDossierAsync(DossierDto dossier);
    }
}