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

        public async Task<PagedResultDto<LogDto>?> GetLogsByVaultIdAsync(Guid vaultId, int pageNumber, int pageSize)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<PagedResultDto<LogDto>>($"api/Log/{vaultId}?pageNumber={pageNumber}&pageSize={pageSize}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des logs : {ex.Message}");
                return null;
            }
        }

        public async Task AddLogAsync(Guid vaultId, string message)
        {
            try
            {
                await _httpClient.PostAsJsonAsync($"api/Log/{vaultId}", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout du log : {ex.Message}");
            }
        }
    }
}
