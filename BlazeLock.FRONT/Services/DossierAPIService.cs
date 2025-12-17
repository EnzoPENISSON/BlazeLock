using BlazeLock.DbLib;
using System.Net.Http.Json;

namespace BlazeLock.FRONT.Services
{
    public class DossierAPIService : IDossierAPIService
    {
        private readonly HttpClient _http;

        public DossierAPIService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<DossierDto>> GetFoldersByCoffreAsync(Guid coffreId)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<DossierDto>>($"api/dossier/coffre/{coffreId}");
                return result ?? new List<DossierDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EntreeAPIService] Error fetching entries for coffre {coffreId}: {ex.Message}");
                return new List<DossierDto>();
            }
        }

        public async Task<bool> CreateDossierAsync(DossierDto dossier)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/dossier", dossier);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DossierAPIService] Error creating dossier: {ex.Message}");
                return false;
            }
        }

    }
}