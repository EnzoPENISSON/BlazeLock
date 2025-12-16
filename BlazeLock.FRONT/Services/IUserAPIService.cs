namespace BlazeLock.FRONT.Services
{
    using System.Threading.Tasks;

    public interface IUserAPIService
    {
        Task InsertUtilisateurAsync(Guid? userId);
    }
}
