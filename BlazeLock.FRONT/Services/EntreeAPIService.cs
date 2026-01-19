using BlazeLock.DbLib;
using System.Net.Http.Json;

namespace BlazeLock.FRONT.Services
{
    public class EntreeAPIService : IEntreeAPIService
    {
        private readonly HttpClient _http;

        public EntreeAPIService(HttpClient http)
        {
            _http = http;
        }
        public async Task<List<EntreeDto>> GetAllByCoffreAsync(Guid coffreId)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<EntreeDto>>($"api/entree/{coffreId}");
                return result ?? new List<EntreeDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EntreeAPIService] Error fetching entries for coffre {coffreId}: {ex.Message}");
                return new List<EntreeDto>();
            }
        }
        public async Task<List<EntreeDto>> GetAllByDossierAsync(Guid idCoffre, Guid dossierId)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<EntreeDto>>($"api/entree/{idCoffre}/dossier/{dossierId}");
                return result ?? new List<EntreeDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EntreeAPIService] Error fetching entries for dossier {dossierId}: {ex.Message}");
                return new List<EntreeDto>();
            }
        }
        public async Task<EntreeDto?> GetByIdAsync(Guid idCoffre, Guid id)
        {
            try
            {
                return await _http.GetFromJsonAsync<EntreeDto>($"api/entree/{idCoffre}/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EntreeAPIService] Error fetching entry {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CreateEntreeAsync(EntreeDto entree)
        {
            try
            {
                // Note: Ensure entree.idCoffre is set before calling this
                var response = await _http.PostAsJsonAsync($"api/entree/{entree.IdCoffre}", entree);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erreur API ({response.StatusCode}): {error}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EntreeAPIService] Error creating entry: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateDossierAsync(Guid idCoffre, Guid targetFolderId, Guid entryId)
        {
            try
            {
                var response = await _http.PostAsync(
                    $"api/entree/{idCoffre}/dossier/{entryId}/{targetFolderId}",
                    null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EntreeAPIService] Error moving entry to dossier: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteEntreeAsync(Guid idCoffre, Guid id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/entree/{idCoffre}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erreur API lors de la suppression ({response.StatusCode}): {error}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EntreeAPIService] Error deleting entry {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<EntreeHistoriqueDto?> GetByIdWithHistoriqueAsync(Guid idCoffre, Guid id)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<EntreeHistoriqueDto>($"api/entree/{idCoffre}/historique/{id}");
                return result;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EntreeAPIService] Error fetching history for entry {id}: {ex.Message}");
                return null;
            }
        }
    }
}