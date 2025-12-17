using BlazeLock.DbLib;
using BlazeLock.FRONT.Components.Forms;
using BlazeLock.FRONT.Components.Types;
using BlazeLock.FRONT.Services;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazeLock.FRONT.ViewModels
{
    public class CoffreDetailViewModel
    {
        private readonly IEntreeAPIService _entreeApi;
        private readonly IDossierAPIService _dossierApi;
        private readonly VaultKeyStore _keyStore;
        private readonly CryptoJs _crypto;
        private readonly NavigationManager _nav;

        public CoffreDetailViewModel(
            IEntreeAPIService entreeApi,
            IDossierAPIService dossierApi,
            VaultKeyStore keyStore,
            CryptoJs crypto, 
            NavigationManager nav)
        {
            _entreeApi = entreeApi;
            _dossierApi = dossierApi;
            _keyStore = keyStore;
            _crypto = crypto;
            _nav = nav;
        }

        public Guid VaultId { get; private set; }
        public Guid? CurrentFolderId { get; private set; }
        public string VaultName { get; private set; } = "";
        public bool HasAccess { get; private set; } = false;
        public bool IsLoading { get; private set; } = true;
        public bool IsFoldersLoading { get; private set; } = true;

        public List<EntreeDto> Entries { get; private set; } = new();
        public List<DossierDto> Folders { get; private set; } = new();

        public CoffreModalType CurrentModal { get; private set; } = CoffreModalType.None;

        public bool IsEntryModalOpen => CurrentModal == CoffreModalType.CreateEntry;
        public bool IsFolderModalOpen => CurrentModal == CoffreModalType.CreateFolder;

        public bool IsProcessing { get; private set; }
        public string ErrorMessage { get; private set; } = "";
        public EntryFormModel NewEntryForm { get; set; } = new();
        public FolderFormModel NewFolderForm { get; set; } = new();

        public async Task InitializeAsync(Guid id, Guid? folderId)
        {
            VaultId = id;
            CurrentFolderId = folderId;
            IsLoading = true;
            IsFoldersLoading = true;

            if (_keyStore.IsUnlocked(VaultId))
            {
                HasAccess = true;
                VaultName = _keyStore.GetName(VaultId) ?? "Coffre";

                await RefreshFoldersAsync();
                await RefreshEntriesAsync(CurrentFolderId);
                //await Task.WhenAll(taskFolders, taskEntries);
            }
            else
            {
                HasAccess = false;
            }
            IsLoading = false;
            IsFoldersLoading = false;
        }

        public async Task RefreshFoldersAsync()
        {
            if (!HasAccess) return;
            try
            {
                Folders = await _dossierApi.GetFoldersByCoffreAsync(VaultId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading entries: {ex.Message}");
            }
        }

        public async Task RefreshEntriesAsync(Guid? folderId)
        {
            if (!HasAccess) return;

            try
            {
                if (folderId == null)
                {
                    Entries = await _entreeApi.GetAllByCoffreAsync(VaultId);
                }else 
                {
                    Entries = await _entreeApi.GetAllByDossierAsync(VaultId, folderId.Value); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading entries: {ex.Message}");
            }
        }

        private void ResetModalState()
        {
            ErrorMessage = "";
            IsProcessing = false;
        }

        public void OpenEntryModal()
        {
            ResetModalState();
            NewEntryForm = new EntryFormModel();
            CurrentModal = CoffreModalType.CreateEntry;
        }

        public void OpenFolderModal()
        {
            ResetModalState();
            NewFolderForm = new FolderFormModel();
            CurrentModal = CoffreModalType.CreateFolder;
        }

        public void CloseModal()
        {
            ResetModalState();
            CurrentModal = CoffreModalType.None;
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

                var passResult = await _crypto.EncryptDataAsync(NewEntryForm.Password, masterKeyBase64);
                dto.Password = Convert.FromBase64String(passResult.CipherText);
                dto.PasswordVi = Convert.FromBase64String(passResult.Iv);
                dto.PasswordTag = Convert.FromBase64String(passResult.Tag);

                var userResult = await _crypto.EncryptDataAsync(NewEntryForm.Username, masterKeyBase64);
                dto.Username = Convert.FromBase64String(userResult.CipherText);
                dto.UsernameVi = Convert.FromBase64String(userResult.Iv);
                dto.UsernameTag = Convert.FromBase64String(userResult.Tag);

                var urlResult = await _crypto.EncryptDataAsync(NewEntryForm.Url, masterKeyBase64);
                dto.Url = Convert.FromBase64String(urlResult.CipherText);
                dto.UrlVi = Convert.FromBase64String(urlResult.Iv);
                dto.UrlTag = Convert.FromBase64String(urlResult.Tag);

                var commResult = await _crypto.EncryptDataAsync(NewEntryForm.Commentaire, masterKeyBase64);
                dto.Commentaire = Convert.FromBase64String(commResult.CipherText);
                dto.CommentaireVi = Convert.FromBase64String(commResult.Iv);
                dto.CommentaireTag = Convert.FromBase64String(commResult.Tag);

                await _entreeApi.CreateEntreeAsync(dto);

                CloseModal();
                await RefreshEntriesAsync(null);
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

        public async Task CreateFolderAsync()
        {
            IsProcessing = true;
            ErrorMessage = "";
            try
            {
                var dto = new DossierDto
                {
                    IdCoffre = VaultId,
                    Libelle = NewFolderForm.Libelle,
                };
                await _dossierApi.CreateDossierAsync(dto);
                CloseModal();
                await RefreshFoldersAsync();
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
}