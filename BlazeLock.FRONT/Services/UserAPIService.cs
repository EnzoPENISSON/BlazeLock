namespace BlazeLock.FRONT.Services
{
    using BlazeLock.DbLib;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

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
            var result = await response.Content.ReadFromJsonAsync<VerifyResponse>();

            bool isValid = result?.IsValid ?? false;

            return isValid;
        }

        public async Task InsertUtilisateurAsync(UtilisateurDto? utilisateur)
        {
            if (utilisateur == null)
            {
                Console.WriteLine("[UserAPIService] userId is null. Aborting.");
                return;
            }

            try
            {
                Console.WriteLine("Envoie");
                var response = await _http.PostAsJsonAsync("/api/utilisateur", utilisateur);

                Console.WriteLine(response);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }

    public class VerifyResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}
