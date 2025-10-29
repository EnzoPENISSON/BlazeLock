namespace BlazeLock.FRONT.Services
{
    using BlazeLock.DbLib;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public interface IUserAPIService
    {
        Task InsertUtilisateurAsync(Guid? userId);
    }

    public class UserAPIService : IUserAPIService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly HttpClient _http; 

        public UserAPIService(AuthenticationStateProvider authenticationStateProvider, HttpClient http)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _http = http;
        }

        public async Task InsertUtilisateurAsync(Guid? userId)
        {
            if (userId == null)
                return;

            var newUser = new UtilisateurDto
            {
                IdUtilisateur = userId.Value,
            };

            try
            {
                await _http.PostAsJsonAsync("/api/Utilisateur", newUser);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }


}
