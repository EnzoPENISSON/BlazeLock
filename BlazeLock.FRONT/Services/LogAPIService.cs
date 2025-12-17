using BlazeLock.DbLib;
using System.Net.Http.Json;

namespace BlazeLock.FRONT.Services
{
    public class LogAPIService : ILogAPIService
    {
        private readonly HttpClient _httpClient;

        public LogAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LogDto>?> GetLogsByVaultIdAsync(Guid vaultId)
        {
            try
            {
                // Assurez-vous que le contrôleur API a une route correspondante, par exemple : api/Log/coffre/{vaultId}
                return await _httpClient.GetFromJsonAsync<List<LogDto>>($"api/Log/coffre/{vaultId}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des logs : {ex.Message}");
                return null;
            }
        }
    }
}
