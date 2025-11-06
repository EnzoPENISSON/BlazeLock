using BlazeLock.DbLib;

namespace BlazeLock.API.Services
{
    public interface ICoffreService
    {
        Task<HashSet<CoffreDto>> GetAllAsync();
        Task<CoffreDto?> GetByIdAsync(Guid id);
        Task<HashSet<CoffreDto>> GetByUtilisateurAsync(Guid id);
        Task AddAsync(CoffreDto dto);
        Task Delete(CoffreDto dto);
    }
}
