namespace BlazeLock.FRONT.Services
{
    public class VaultKeyStore
    {
        // Stores keys (Base64) for unlocked safes: <CoffreId, KeyBase64>
        private readonly Dictionary<Guid, string> _unlockedKeys = new();

        // Stores decrypted names for display: <CoffreId, Name>
        private readonly Dictionary<Guid, string> _decryptedNames = new();

        public bool IsUnlocked(Guid id) => _unlockedKeys.ContainsKey(id);

        public string? GetKey(Guid id) => _unlockedKeys.TryGetValue(id, out var key) ? key : null;
        public string? GetName(Guid id) => _decryptedNames.TryGetValue(id, out var name) ? name : null;

        public void Store(Guid id, string keyBase64, string decryptedName)
        {
            _unlockedKeys[id] = keyBase64;
            _decryptedNames[id] = decryptedName;
        }

        public void Clear()
        {
            _unlockedKeys.Clear();
            _decryptedNames.Clear();
        }
    }
}