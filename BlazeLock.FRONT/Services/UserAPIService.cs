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

        public async Task<bool> CreateCoffreAsync(CoffreDto coffre)
        {
            var response = await _http.PostAsJsonAsync("api/coffre", coffre);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erreur API ({response.StatusCode}): {error}");
            }

            return true;
        }

        public async Task<List<CoffreDto>> GetMyCoffresAsync()
        {
            var response = await _http.GetFromJsonAsync<List<CoffreDto>>("api/coffre/mine");
            return response ?? new List<CoffreDto>();
        }

        public async Task<bool> VerifyMasterKeyAsync(CoffreDto coffre)
        {
            var response = await _http.PostAsJsonAsync("api/coffre/verify-password", coffre);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erreur API ({response.StatusCode}): {error}");
            }
            bool isValid = await response.Content.ReadFromJsonAsync<bool>();

            return isValid;
        }

        public async Task InsertUtilisateurAsync(Guid? userId)
        {
            if (userId == null)
            {
                Console.WriteLine("[UserAPIService] userId is null. Aborting.");
                return;
            }

            var newUser = new UtilisateurDto
            {
                IdUtilisateur = userId.Value,
            };

            try
            {
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(newUser);
                
                var response = await _http.PostAsJsonAsync("/api/utilisateur", newUser);

                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FRONT.UserAPIService] EXCEPTION: {ex.Message}");
            }
        }
    }
}
