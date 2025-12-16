using BlazeLock.DbLib;

namespace BlazeLock.API.Services
{
    public interface IDossierService
    {
        Task<HashSet<DossierDto>> GetAllAsync();
        Task<DossierDto?> GetByIdAsync(Guid id);
        Task<HashSet<DossierDto>> GetByCoffreAsync(Guid id);
        Task AddAsync(DossierDto dto);
        Task Delete(DossierDto dto);
    }
}
