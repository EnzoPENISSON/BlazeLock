using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace BlazeLock.FRONT.Services
{
    public class CryptoJs
    {
        private readonly IJSRuntime _js;

        public CryptoJs(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<EncryptionResult> EncryptDataAsync(string clearText, string key)
        {
            if (string.IsNullOrEmpty(clearText)) clearText = "";

            var res = await _js.InvokeAsync<EncryptionResult>("blazeCrypto.encryptData", clearText, key);
            return res;
        }

        public async Task<string?> DecryptDataAsync(byte[]? cipherText, byte[]? iv, byte[]? tag, string keyBase64)
        {
            if (cipherText == null || iv == null || tag == null || string.IsNullOrEmpty(keyBase64))
                return null;

            try
            {

                int totalLength = iv.Length + cipherText.Length + tag.Length;
                byte[] combined = new byte[totalLength];

                Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);

                Buffer.BlockCopy(cipherText, 0, combined, iv.Length, cipherText.Length);

                Buffer.BlockCopy(tag, 0, combined, iv.Length + cipherText.Length, tag.Length);

                string combinedBase64 = Convert.ToBase64String(combined);

                var result = await _js.InvokeAsync<string>("blazeCrypto.decryptData", combinedBase64, keyBase64);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption Error in C# Wrapper: {ex.Message}");
                return null;
            }
        }
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