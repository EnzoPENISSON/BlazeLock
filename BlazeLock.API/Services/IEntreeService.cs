using BlazeLock.DbLib;

namespace BlazeLock.API.Services
{
    public interface IEntreeService
    {
        Task<HashSet<EntreeDto>> GetAllAsync();
        Task<HashSet<EntreeDto>> GetAllByDossierAsync(Guid IdDossier);
        Task<EntreeDto?> GetByIdAsync(Guid idEntree);
        Task<EntreeHistoriqueDto?> GetByIdWithHistoriaqueAsync(Guid idEntree);
        Task AddAsync(EntreeDto dto);
        //Task Delete(EntreeDto dto);

    }
}
