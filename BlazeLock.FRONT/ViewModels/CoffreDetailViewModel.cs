using BlazeLock.DbLib;
using BlazeLock.FRONT.Services;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazeLock.FRONT.ViewModels
{
    public class CoffreDetailViewModel
    {
        private readonly IEntreeAPIService _entreeApi;
        private readonly VaultKeyStore _keyStore;
        private readonly CryptoJs _crypto;
        private readonly NavigationManager _nav;

        public CoffreDetailViewModel(
            IEntreeAPIService entreeApi,
            VaultKeyStore keyStore,
            CryptoJs crypto, 
            NavigationManager nav)
        {
            _entreeApi = entreeApi;
            _keyStore = keyStore;
            _crypto = crypto;
            _nav = nav;
        }

        public Guid VaultId { get; private set; }
        public string VaultName { get; private set; } = "";
        public bool HasAccess { get; private set; } = false;
        public bool IsLoading { get; private set; } = true;

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

                var dto = new EntreeDto
                {
                    IdDossier = Guid.Empty,
                    idCoffre = VaultId,
                    Libelle = NewEntryForm.Libelle,
                    DateCreation = DateTime.UtcNow,
                    DateUpdate = DateTime.UtcNow
                };

                // --- ENCRYPTION LOGIC USING NEW SERVICE ---

                // 1. Password
                var passResult = await _crypto.EncryptDataAsync(NewEntryForm.Password, masterKeyBase64);
                dto.Password = Convert.FromBase64String(passResult.CipherText);
                dto.PasswordVi = Convert.FromBase64String(passResult.Iv);
                dto.PasswordTag = Convert.FromBase64String(passResult.Tag);

                // 2. Username
                var userResult = await _crypto.EncryptDataAsync(NewEntryForm.Username, masterKeyBase64);
                dto.Username = Convert.FromBase64String(userResult.CipherText);
                dto.UsernameVi = Convert.FromBase64String(userResult.Iv);
                dto.UsernameTag = Convert.FromBase64String(userResult.Tag);

                // 3. URL
                var urlResult = await _crypto.EncryptDataAsync(NewEntryForm.Url, masterKeyBase64);
                dto.Url = Convert.FromBase64String(urlResult.CipherText);
                dto.UrlVi = Convert.FromBase64String(urlResult.Iv);
                dto.UrlTag = Convert.FromBase64String(urlResult.Tag);

                // 4. Commentaire (Fixed Variable Name Bug)
                var commResult = await _crypto.EncryptDataAsync(NewEntryForm.Commentaire, masterKeyBase64);
                dto.Commentaire = Convert.FromBase64String(commResult.CipherText);
                dto.CommentaireVi = Convert.FromBase64String(commResult.Iv);
                dto.CommentaireTag = Convert.FromBase64String(commResult.Tag);

                // --- SAVE TO API ---
                await _entreeApi.CreateEntreeAsync(dto);

                CloseModal();
                await RefreshEntriesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur : " + ex.Message;
                Console.WriteLine(ex);
            }
            finally
            {
                IsProcessing = false;
            }
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
        public string Commentaire { get; set; } = "";
    }
}