using BlazeLock.DbLib;
using BlazeLock.FRONT.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazeLock.FRONT.ViewModels
{
    public class HomeViewModel
    {
        private readonly IJSRuntime _js;
        private readonly UserAPIService _api;
        private readonly VaultKeyStore _keyStore;
        private readonly NavigationManager _nav;

        public HomeViewModel(IJSRuntime js, UserAPIService api, VaultKeyStore keyStore, NavigationManager nav)
        {
            _js = js;
            _api = api;
            _keyStore = keyStore;
            _nav = nav;
        }

        public List<CoffreDto> MyCoffres { get; private set; } = new();
        public bool IsLoading { get; private set; } = true;

        public bool IsCreating { get; private set; }
        public CoffreDto? SelectedCoffre { get; private set; }
        public bool IsProcessing { get; private set; }

        public string FeedbackMessage { get; private set; } = "";
        public bool IsSuccess { get; private set; }

        public string NewLibelle { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string UnlockPassword { get; set; } = "";

        public bool IsUnlocked(Guid id) => _keyStore.IsUnlocked(id);
        public string GetDisplayName(Guid id) => _keyStore.GetName(id) ?? "";
        public bool HasCoffres => MyCoffres.Any();

        public async Task LoadDataAsync()
        {
            IsLoading = true;
            try {
                MyCoffres = await _api.GetMyCoffresAsync();
            }
            catch { /* Handle error */ }
            finally {
                IsLoading = false; 
            }
        }

        public void OpenCreateModal()
        {
            IsCreating = true;
            NewLibelle = "";
            NewPassword = "";
            FeedbackMessage = "";
        }

        public void CloseModals()
        {
            IsCreating = false;
            SelectedCoffre = null;
            FeedbackMessage = "";
            UnlockPassword = "";
        }

        public void SelectCoffreToUnlock(CoffreDto coffre)
        {
            SelectedCoffre = coffre;
            UnlockPassword = "";
            FeedbackMessage = "";
        }

        public async Task CreateCoffreAsync()
        {
            if (string.IsNullOrWhiteSpace(NewLibelle) || string.IsNullOrWhiteSpace(NewPassword))
            {
                SetFeedback(false, "Veuillez remplir tous les champs.");
                return;
            }

            IsProcessing = true;
            try
            {
                var dto = new CoffreDto
                {
                    Libelle = NewLibelle,
                    ClearPassword = NewPassword
                };

                await _api.CreateCoffreAsync(dto);

                await LoadDataAsync();
                CloseModals();
            }
            catch (Exception ex)
            {
                SetFeedback(false, "Erreur : " + ex.Message);
            }
            finally { IsProcessing = false; }
        }

        public async Task UnlockSelectedAsync()
        {
            if (SelectedCoffre == null || string.IsNullOrWhiteSpace(UnlockPassword)) return;

            IsProcessing = true;
            FeedbackMessage = "";

            try
            {
                var coffreToUnlock = new CoffreDto();
                coffreToUnlock.IdCoffre = SelectedCoffre.IdCoffre;
                coffreToUnlock.ClearPassword = UnlockPassword;
                bool isUnlock = await _api.VerifyMasterKeyAsync(coffreToUnlock);

                if (!isUnlock)
                {
                    SetFeedback(false, "Mot de passe incorrect.");
                    return;
                }

                var saltB64 = Convert.ToBase64String(SelectedCoffre.Salt);
                var keyB64 = await _js.InvokeAsync<string>("blazeCrypto.deriveKey", UnlockPassword, saltB64);
                _keyStore.Store(SelectedCoffre.IdCoffre, keyB64, SelectedCoffre.Libelle);

                CloseModals();
            }
            catch (Exception ex)
            {
                SetFeedback(false, "Erreur : " + ex.Message);
            }
            finally { IsProcessing = false; }
        }

        private void SetFeedback(bool success, string message)
        {
            IsSuccess = success;
            FeedbackMessage = message;
        }
    }
}