namespace BlazeLock.FRONT.Components.Services
{
    using BlazeLock.DbLib;
    using Microsoft.AspNetCore.Components.Authorization;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<Guid?> GetUserIdAsync();
        Task<string?> GetUsernameAsync();
        Task InsertUtilisateurAsync(Guid? userId);
    }

    public class UserService : IUserService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly HttpClient _http; // <-- add this

        public UserService(AuthenticationStateProvider authenticationStateProvider, HttpClient http)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _http = http;
        }

        public async Task<Guid?> GetUserIdAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                var objectIdClaim = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
                return objectIdClaim != null && Guid.TryParse(objectIdClaim.Value, out Guid userId) ? userId : null;
            }

            return null;
        }

        public async Task<string?> GetUsernameAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var usernameClaim = user.FindFirst("preferred_username") ?? user.FindFirst("upn");
                return usernameClaim?.Value;
            }

            return null;
        }

        public async Task InsertUtilisateurAsync(Guid? userId)
        {
            if (userId == null)
                return;

            var newUser = new UtilisateurDto
            {
                IdUtilisateur = userId.Value,
            };

            var response = await _http.PostAsJsonAsync("/api/Utilisateur", newUser);

            response.EnsureSuccessStatusCode();
        }
    }


}
