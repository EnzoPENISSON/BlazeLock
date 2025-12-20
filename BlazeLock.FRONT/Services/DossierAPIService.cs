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
                var result = await _http.GetFromJsonAsync<List<DossierDto>>($"api/dossier/{coffreId}");
                return result ?? new List<DossierDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DossierAPIService] Error fetching folders: {ex.Message}");
                return new List<DossierDto>();
            }
        }

        public async Task<DossierDto?> GetDossierByIdAsync(Guid coffreId, Guid dossierId)
        {
            try
            {
                return await _http.GetFromJsonAsync<DossierDto>($"api/dossier/{coffreId}/{dossierId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DossierAPIService] Error fetching dossier {dossierId}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateDossierAsync(DossierDto dossier)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"api/dossier/{dossier.IdCoffre}", dossier);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DossierAPIService] Error creating dossier: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateDossierAsync(DossierDto dossier)
        {
            try
            {
                var response = await _http.PutAsJsonAsync(
                    $"api/dossier/{dossier.IdCoffre}/{dossier.IdDossier}",
                    dossier);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DossierAPIService] Error updating dossier: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteDossierAsync(Guid coffreId, Guid dossierId)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/dossier/{coffreId}/{dossierId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DossierAPIService] Error deleting dossier: {ex.Message}");
                return false;
            }
        }
    }
}