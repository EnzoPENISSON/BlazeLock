using BlazeLock.DbLib;

namespace BlazeLock.API.Services
{
    public interface IPartageService
    {
        Task<HashSet<PartageDto>> GetAllAsync();
        Task<HashSet<PartageDto>> GetByCoffreAsync(Guid id);
        Task<HashSet<PartageDto>> GetByUtilisateurAsync(Guid id);
        Task<bool> HasAccess(Guid coffreId, Guid userId);
        Task AddAsync(PartageDto dto);
        Task Delete(PartageDto dto);

    }
}
