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

        // --- NEW DECRYPTION METHOD ---
        public async Task<string?> DecryptDataAsync(byte[]? cipherText, byte[]? iv, byte[]? tag, string keyBase64)
        {
            // 1. Validation: If any part is missing, we can't decrypt
            if (cipherText == null || iv == null || tag == null || string.IsNullOrEmpty(keyBase64))
                return null;

            try
            {
                // 2. Prepare the single byte array expected by your JS: [IV (12)] + [Cipher] + [Tag]
                // Your JS does: const iv = bytes.slice(0,12); const data = bytes.slice(12);

                int totalLength = iv.Length + cipherText.Length + tag.Length;
                byte[] combined = new byte[totalLength];

                // Copy IV (First 12 bytes)
                Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);

                // Copy CipherText
                Buffer.BlockCopy(cipherText, 0, combined, iv.Length, cipherText.Length);

                // Copy Tag (Appended at the end for AES-GCM)
                Buffer.BlockCopy(tag, 0, combined, iv.Length + cipherText.Length, tag.Length);

                // 3. Convert to Base64 string
                string combinedBase64 = Convert.ToBase64String(combined);

                // 4. Call JS
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