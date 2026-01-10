namespace BlazeLock.FRONT.Services
{
    using BlazeLock.DbLib;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IUserAPIService
    {
        Task<bool> CreateCoffreAsync(CoffreDto coffre);
        Task DeleteCoffreAsync(Guid idCoffre);
        Task<List<CoffreDto>> GetMyCoffresAsync();
        Task<bool> VerifyMasterKeyAsync(CoffreDto coffre);
        Task InsertUtilisateurAsync(UtilisateurDto? utilisateur);
        Task<List<UtilisateurDto>> SearchUtilisateursAsync(string term);

    }
}
