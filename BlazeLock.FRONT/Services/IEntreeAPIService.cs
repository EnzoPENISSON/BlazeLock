using BlazeLock.DbLib;
using System.Net.Http.Json;

namespace BlazeLock.FRONT.Services
{
    public interface IEntreeAPIService
    {
        Task<List<EntreeDto>> GetAllByCoffreAsync(Guid coffreId);
        Task<List<EntreeDto>> GetAllByDossierAsync(Guid idCoffre, Guid dossierId);
        Task<EntreeDto?> GetByIdAsync(Guid id);
        Task<bool> CreateEntreeAsync(EntreeDto entree);
        Task<bool> DeleteEntreeAsync(Guid id);
    }
}