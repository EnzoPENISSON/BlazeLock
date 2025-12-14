using BlazeLock.DbLib;

namespace BlazeLock.API.Services
{
    public interface IUtilisateurService
    {
        Task<HashSet<UtilisateurDto>> GetAllAsync();
        Task<UtilisateurDto?> GetByIdAsync(Guid id);
        Task AddAsync(UtilisateurDto dto);
        Task<bool> ExistsAsync(Guid id);
    }
}
