using BlazeLock.DbLib;

namespace BlazeLock.FRONT.Services
{
    public interface IPartageAPIService
    {
        Task AddPartageAsync(PartageDto partageDto);
        Task<List<PartageDto>> GetPartagesByCoffreIdAsync(Guid coffreId);
        Task DeletePartageAsync(PartageDto partageDto);
    }
}