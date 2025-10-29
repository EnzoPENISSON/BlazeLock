using DbLib;

namespace BlazeLock.API.Services
{
    public interface IUtilisateurService
    {
        Task<HashSet<UtilisateurDto>> GetAllAsync();
        Task<UtilisateurDto?> GetByIdAsync(Guid id);
        Task AddUtilisateurAsync(UtilisateurDto dto);
    }
}
