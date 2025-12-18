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
                var result = await _http.GetFromJsonAsync<List<EntreeDto>>($"api/entree/coffre/{coffreId}");
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
                var result = await _http.GetFromJsonAsync<List<EntreeDto>>($"api/entree/dossier/{idCoffre}/{dossierId}");
                return result ?? new List<EntreeDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"[EntreeAPIService] Error fetching entries for dossier {dossierId}: {ex.Message}");
                return new List<EntreeDto>();
            }
        }

        public async Task<EntreeDto?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _http.GetFromJsonAsync<EntreeDto>($"api/entree/{id}");
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
                var response = await _http.PostAsJsonAsync("api/entree", entree);

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

        public async Task<bool> UpdateDossierAsync(Guid targetFolderId, Guid entryId)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"api/entree/dossier/{entryId}/{targetFolderId}", entryId);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DossierAPIService] Error creating dossier: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteEntreeAsync(Guid id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/entree/{id}");

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
    }
}