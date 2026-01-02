namespace BlazeLock.FRONT.Services
{
    using BlazeLock.DbLib;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IUserAPIService
    {
        Task InsertUtilisateurAsync(UtilisateurDto? utilisateur);
        Task<List<UtilisateurDto>> SearchUtilisateursAsync(string term);
    }
}
