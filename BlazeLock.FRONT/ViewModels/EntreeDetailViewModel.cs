using BlazeLock.DbLib;
using BlazeLock.FRONT.Services;

namespace BlazeLock.FRONT.ViewModels
{
    public class EntreeDetailViewModel
    {
        private readonly HttpClient _http;
        private readonly VaultKeyStore _keyStore;
        private readonly CryptoJs _crypto;

        public EntreeDetailViewModel(HttpClient http, VaultKeyStore keyStore, CryptoJs crypto)
        {
            _http = http;
            _keyStore = keyStore;
            _crypto = crypto;
        }  
    }
}