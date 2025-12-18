namespace BlazeLock.FRONT.Services
{
    using BlazeLock.DbLib;
    using System.Threading.Tasks;

    public interface IUserAPIService
    {
        Task InsertUtilisateurAsync(UtilisateurDto? utilisateur);
    }
}
