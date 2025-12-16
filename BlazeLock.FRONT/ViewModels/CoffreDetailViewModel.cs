using BlazeLock.DbLib;
using BlazeLock.FRONT.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazeLock.FRONT.ViewModels
{
    public class CoffreDetailViewModel
    {
        private readonly IEntreeAPIService _entreeApi;
        private readonly VaultKeyStore _keyStore;
        private readonly IJSRuntime _js;
        private readonly NavigationManager _nav;

        public CoffreDetailViewModel(
            IEntreeAPIService entreeApi,
            VaultKeyStore keyStore,
            IJSRuntime js,
            NavigationManager nav)
        {
            _entreeApi = entreeApi;
            _keyStore = keyStore;
            _js = js;
            _nav = nav;
        }

        // --- State Properties ---
        public Guid VaultId { get; private set; }
        public string VaultName { get; private set; } = "";
        public bool HasAccess { get; private set; } = false;
        public bool IsLoading { get; private set; } = true;

        // List of entries in the vault
        public List<EntreeDto> Entries { get; private set; } = new();

        // --- Creation Modal State ---
        public bool IsCreating { get; private set; }
        public bool IsProcessing { get; private set; }
        public string ErrorMessage { get; private set; } = "";

        public EntryFormModel NewEntryForm { get; set; } = new();

        public async Task InitializeAsync(Guid id)
        {
            VaultId = id;
            IsLoading = true;

            if (_keyStore.IsUnlocked(VaultId))
            {
                HasAccess = true;
                VaultName = _keyStore.GetName(VaultId) ?? "Coffre";
                await RefreshEntriesAsync();
            }
            else
            {
                HasAccess = false;
                IsLoading = false;
            }
        }

        public async Task RefreshEntriesAsync()
        {
            if (!HasAccess) return;

            IsLoading = true;
            try
            {
                Entries = await _entreeApi.GetAllByCoffreAsync(VaultId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading entries: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void OpenCreateModal()
        {
            NewEntryForm = new EntryFormModel();
            ErrorMessage = "";
            IsCreating = true;
        }

        public void CloseModal()
        {
            IsCreating = false;
            ErrorMessage = "";
        }

        public async Task CreateEntryAsync()
        {
            IsProcessing = true;
            ErrorMessage = "";

            try
            {
                string? masterKeyBase64 = _keyStore.GetKey(VaultId);
                if (string.IsNullOrEmpty(masterKeyBase64))
                {
                    ErrorMessage = "La clé du coffre est introuvable. Veuillez le déverrouiller à nouveau.";
                    return;
                }

                Console.WriteLine("Master Key (Base64): " + masterKeyBase64);

                // 2. Prepare the DTO
                var dto = new EntreeDto
                {
                    IdDossier = Guid.Empty,
                    Libelle = NewEntryForm.Libelle,
                    DateCreation = DateTime.UtcNow,
                    DateUpdate = DateTime.UtcNow
                };

                // 3. Encrypt Password
                var passResult = await EncryptFieldJS(NewEntryForm.Password, masterKeyBase64);
                dto.Password = Convert.FromBase64String(passResult.CipherText);
                dto.PasswordVi = Convert.FromBase64String(passResult.Iv);
                dto.PasswordTag = Convert.FromBase64String(passResult.Tag);

                // 4. Encrypt Username
                if (!string.IsNullOrWhiteSpace(NewEntryForm.Username))
                {
                    var userResult = await EncryptFieldJS(NewEntryForm.Username, masterKeyBase64);
                    dto.Username = Convert.FromBase64String(userResult.CipherText);
                    dto.UsernameVi = Convert.FromBase64String(userResult.Iv);
                    dto.UsernameTag = Convert.FromBase64String(userResult.Tag);
                }

                // 5. Encrypt URL
                if (!string.IsNullOrWhiteSpace(NewEntryForm.Url))
                {
                    var urlResult = await EncryptFieldJS(NewEntryForm.Url, masterKeyBase64);
                    dto.Url = Convert.FromBase64String(urlResult.CipherText);
                    dto.UrlVi = Convert.FromBase64String(urlResult.Iv);
                    dto.UrlTag = Convert.FromBase64String(urlResult.Tag);
                }

                // 6. Send to API
                await _entreeApi.CreateEntreeAsync(dto);

                // 7. Success
                CloseModal();
                await RefreshEntriesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur : " + ex.Message;
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task<EncryptionResult> EncryptFieldJS(string clearText, string key)
        {
            var res = await _js.InvokeAsync<EncryptionResult>("blazeCrypto.encryptData", clearText, key);
            Console.WriteLine("Encrypted Password Result: " + res);
            Console.WriteLine(res.CipherText + " " + res.Iv + " " + res.Tag);
            return res;
        }

        public void NavigateHome()
        {
            _nav.NavigateTo("/");
        }
    }


    public class EntryFormModel
    {
        [Required(ErrorMessage = "Le titre est requis")]
        public string Libelle { get; set; } = "";

        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string Password { get; set; } = "";

        public string Username { get; set; } = "";
        public string Url { get; set; } = "";
    }

    public class EncryptionResult
    {
        [JsonPropertyName("cipherText")] 
        public string CipherText { get; set; } = "";

        [JsonPropertyName("iv")]
        public string Iv { get; set; } = "";

        [JsonPropertyName("tag")]
        public string Tag { get; set; } = "";
    }
}