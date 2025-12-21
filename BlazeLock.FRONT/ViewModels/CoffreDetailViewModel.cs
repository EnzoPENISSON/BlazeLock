using BlazeLock.DbLib;
using BlazeLock.FRONT.Components.Forms;
using BlazeLock.FRONT.Components.Types;
using BlazeLock.FRONT.Services;
using Microsoft.AspNetCore.Components;

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
        public Guid? CurrentFolderId { get; set; }
        public Guid _currentEntryId;

        public string VaultName { get; private set; } = "";
        public bool HasAccess { get; private set; } = false;
        public bool IsLoading { get; private set; } = true;
        public bool IsFoldersLoading { get; private set; } = true;

        public Guid DefaultFolderId { get; set; }

        public List<EntreeDto> Entries { get; private set; } = new();
        public List<DossierDto> Folders { get; private set; } = new();

        public string CurrentEntryTitle { get; private set; } = "";

        public CoffreModalType CurrentModal { get; private set; } = CoffreModalType.None;

        public bool IsEntryModalOpen => CurrentModal == CoffreModalType.CreateEntry || CurrentModal == CoffreModalType.UpdateEntry;
        public bool IsFolderModalOpen => CurrentModal == CoffreModalType.CreateFolder;
        public bool IsMoveModalOpen => CurrentModal == CoffreModalType.MoveEntry;

        public bool IsProcessing { get; private set; }
        public string ErrorMessage { get; private set; } = "";
        public EntryFormModel NewEntryForm { get; set; } = new();
        public FolderFormModel NewFolderForm { get; set; } = new();

        [Inject]
        public NavigationManager Nav { get; set; }

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

                DefaultFolderId = Folders.FirstOrDefault(f => f.Libelle == "Default")?.IdDossier ?? Guid.Empty;

                await RefreshEntriesAsync(CurrentFolderId);
            }
            else
            {
                HasAccess = false;
            }
            IsLoading = false;
            IsFoldersLoading = false;
        }

        public async Task ReloadEntrieesAsync(Guid? folderId)
        {
            CurrentFolderId = folderId;
            IsLoading = true;

            if (_keyStore.IsUnlocked(VaultId))
            {
                HasAccess = true;
                VaultName = _keyStore.GetName(VaultId) ?? "Coffre";

                await RefreshEntriesAsync(CurrentFolderId);
            }
            else
            {
                HasAccess = false;
            }

            IsLoading = false;
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

                for (int i = 0; i < Entries.Count; i++)
                {
                    var entry = Entries[i];
                    var domain = await GetDecryptedDomainAsync(entry);
                    if (!string.IsNullOrEmpty(domain))
                    {
                        Entries[i].LogoUrl = $"https://www.google.com/s2/favicons?domain={domain}&sz=64";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading entries: {ex.Message}");
            }
        }

        public async Task<string> GetDecryptedDomainAsync(EntreeDto entry)
        {
            if (entry == null) return "";

            try
            {
                var key = _keyStore.GetKey(entry.idCoffre);
                if (string.IsNullOrEmpty(key)) return "";

                var url = await _crypto.DecryptDataAsync(entry.Url, entry.UrlVi, entry.UrlTag, key);

                if (string.IsNullOrEmpty(url)) return "";

                if (!url.StartsWith("http")) url = "https://" + url;
                return new Uri(url).Host;
            }
            catch
            {
                return "";
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

        public async Task UpdateFolderAsync()
        {
            IsProcessing = true;
            ErrorMessage = "";
            try
            {
                var dto = new DossierDto
                {
                    IdDossier = NewFolderForm.Id,
                    IdCoffre = VaultId,
                    Libelle = NewFolderForm.Libelle
                };

                await _dossierApi.UpdateDossierAsync(dto);

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

        public void OpenUpdateFolderModal(DossierDto folderToEdit)
        {
            NewFolderForm = new FolderFormModel
            {
                Id = folderToEdit.IdDossier,
                Libelle = folderToEdit.Libelle
            };

            ErrorMessage = "";
            IsProcessing = false;

            CurrentModal = CoffreModalType.UpdateFolder;
        }

        public void OpenDeleteFolderModal(DossierDto folder)
        {
            NewFolderForm = new FolderFormModel
            {
                Id = folder.IdDossier,
                Libelle = folder.Libelle
            };
            ErrorMessage = "";
            IsProcessing = false;
            CurrentModal = CoffreModalType.DeleteFolder;
        }

        public async Task OpenEditEntryModal(EntreeDto entry)
        {
            if (IsProcessing) return;
            IsProcessing = true;

            try
            {
                string? masterKeyBase64 = _keyStore.GetKey(VaultId);
                if (masterKeyBase64 == null) return;

                _currentEntryId = entry.IdEntree;

                var password = await _crypto.DecryptDataAsync(entry.Password, entry.PasswordVi, entry.PasswordTag, masterKeyBase64);
                var username = await _crypto.DecryptDataAsync(entry.Username, entry.UsernameVi, entry.UsernameTag, masterKeyBase64);
                var url = await _crypto.DecryptDataAsync(entry.Url, entry.UrlVi, entry.UrlTag, masterKeyBase64);
                var comment = await _crypto.DecryptDataAsync(entry.Commentaire, entry.CommentaireVi, entry.CommentaireTag, masterKeyBase64);

                NewEntryForm = new EntryFormModel
                {
                    Libelle = entry.Libelle,
                    Password = password,
                    Username = username,
                    Url = url,
                    Commentaire = comment
                };

                ErrorMessage = "";
                CurrentModal = CoffreModalType.UpdateEntry;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ErrorMessage = "Erreur lors du déchiffrement.";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public async Task SaveEntryAsync()
        {
            Console.WriteLine(CurrentModal);
            if (CurrentModal == CoffreModalType.CreateEntry)
                await CreateEntryAsync();
            else if (CurrentModal == CoffreModalType.UpdateEntry)
                await UpdateEntryAsync();
        }

        private async Task UpdateEntryAsync()
        {
            IsProcessing = true;
            ErrorMessage = "";
            try
            {
                string? masterKeyBase64 = _keyStore.GetKey(VaultId);
                if (string.IsNullOrEmpty(masterKeyBase64)) return;

                var dto = await _entreeApi.GetByIdAsync(VaultId,_currentEntryId);

                dto.Libelle = NewEntryForm.Libelle;

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

                dto.idCoffre = VaultId;
                dto.IdDossier = CurrentFolderId ?? Guid.Empty;

                await _entreeApi.CreateEntreeAsync(dto);

                CloseModal();
                await RefreshEntriesAsync(CurrentFolderId);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur : " + ex.Message;
            }
            finally { IsProcessing = false; }
        }

        public void OpenMoveEntryModal(EntreeDto entry)
        {
            _currentEntryId = entry.IdEntree;
            ErrorMessage = "";
            CurrentModal = CoffreModalType.MoveEntry;
        }

        public async Task MoveEntryToFolderAsync(Guid targetFolderId, Guid entryId)
        {
            IsProcessing = true;
            try
            {
                await _entreeApi.UpdateDossierAsync(VaultId,targetFolderId,entryId);
                CloseModal();

                Console.WriteLine(targetFolderId);
                Console.WriteLine(entryId);

                string targetUrl;

                if (targetFolderId == DefaultFolderId)
                {
                    targetUrl = $"/coffre/{VaultId}";
                }
                else
                {
                    targetUrl = $"/coffre/{VaultId}/{targetFolderId}";
                }

                _nav.NavigateTo(targetUrl);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur: " + ex.Message;
            }
            finally { IsProcessing = false; }
        }
    
        public void OpenDeleteEntryModal(EntreeDto entry)
        {
            _currentEntryId = entry.IdEntree;
            CurrentEntryTitle = entry.Libelle;
            ErrorMessage = "";
            CurrentModal = CoffreModalType.DeleteEntry;
        }

        public async Task DeleteEntryAsync()
        {
            IsProcessing = true;
            try
            {
                await _entreeApi.DeleteEntreeAsync(VaultId,_currentEntryId);

                CloseModal();
                await RefreshEntriesAsync(CurrentFolderId);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors de la suppression : " + ex.Message;
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public async Task DeleteFolderAsync()
        {
            IsProcessing = true;
            try
            {
                await _dossierApi.DeleteDossierAsync(VaultId, NewFolderForm.Id);

                CloseModal();
                _nav.NavigateTo($"/coffre/{VaultId}");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors de la suppression : " + ex.Message;
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}