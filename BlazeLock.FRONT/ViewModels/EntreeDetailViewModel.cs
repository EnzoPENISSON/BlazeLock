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

        public string LogoUrl { get; private set; }

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

        public class LogoApiResult
        {
            public string Name { get; set; }
            public string Domain { get; set; }
            public string Logo_url { get; set; }
        }
    }
}