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
            // 1. Log the Input Parameter
            Console.WriteLine($"[UserAPIService] InsertUtilisateurAsync START. Param userId: {userId}");

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
                // 2. Log before sending
                // Serializing to JSON helps verify exactly what you are sending to the API
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(newUser);
                Console.WriteLine($"[UserAPIService] Sending POST to /api/utilisateur. Payload: {jsonPayload}");

                // 3. Capture the response object!
                var response = await _http.PostAsJsonAsync("/api/utilisateur", newUser);

                // 4. Log the HTTP Status Code (200, 401, 500, etc.)
                Console.WriteLine($"[UserAPIService] HTTP Response Code: {response.StatusCode}");

                // 5. If it failed, read the body to see the specific error message from the API
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[UserAPIService] ❌ ERROR BODY: {errorContent}");
                }
                else
                {
                    Console.WriteLine("[UserAPIService] ✅ Success.");
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                Console.WriteLine("[UserAPIService] 🔒 Token not available. Redirecting to login...");
                exception.Redirect();
            }
            catch (Exception ex)
            {
                // 6. Catch generic connection errors (DNS, Server down, CORS)
                Console.WriteLine($"[UserAPIService] 💥 EXCEPTION: {ex.Message}");
            }
        }
    }
}
