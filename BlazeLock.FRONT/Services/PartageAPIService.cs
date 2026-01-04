using BlazeLock.DbLib;
using System.Net.Http;
using System.Net.Http.Json;

namespace BlazeLock.FRONT.Services
{
    public class PartageAPIService : IPartageAPIService
    {
        private readonly HttpClient _httpClient;

        public PartageAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddPartageAsync(PartageDto partage)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/partage/{partage.IdCoffre}", partage);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeletePartageAsync(PartageDto partage)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/partage/{partage.IdCoffre}")
            {
                Content = JsonContent.Create(partage)
            };
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<PartageDto>> GetPartagesByCoffreIdAsync(Guid coffreId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<PartageDto>>($"api/partage/coffre/{coffreId}");
            return response ?? new List<PartageDto>();
        }
    }
}