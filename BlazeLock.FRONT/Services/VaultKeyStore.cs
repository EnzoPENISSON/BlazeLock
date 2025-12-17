namespace BlazeLock.FRONT.Services
{
    public class VaultKeyStore
    {
        private readonly Dictionary<Guid, string> _unlockedKeys = new();
        private readonly Dictionary<Guid, string> _decryptedNames = new();

        private readonly Dictionary<Guid, DateTime> _lastAccessTime = new();

        private readonly TimeSpan _timeoutDuration = TimeSpan.FromMinutes(10);

        public bool IsUnlocked(Guid id)
        {
            CheckExpiration(id);

            if (_unlockedKeys.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public string? GetKey(Guid id)
        {
            CheckExpiration(id);

            if (_unlockedKeys.TryGetValue(id, out var key))
            {
                return key;
            }
            return null;
        }

        public string? GetName(Guid id)
        {
            return _decryptedNames.TryGetValue(id, out var name) ? name : null;
        }

        public void Store(Guid id, string keyBase64, string decryptedName)
        {
            _unlockedKeys[id] = keyBase64;
            _decryptedNames[id] = decryptedName;
            RefreshSession(id);
        }

        public void SetName(Guid id, string decryptedName)
        {
            if (_decryptedNames.ContainsKey(id))
            {
                _decryptedNames[id] = decryptedName;
            }
        }

        public void Clear()
        {
            _unlockedKeys.Clear();
            _decryptedNames.Clear();
            _lastAccessTime.Clear();
        }

        private void RefreshSession(Guid id)
        {
            _lastAccessTime[id] = DateTime.UtcNow;
        }

        private void CheckExpiration(Guid id)
        {
            if (_lastAccessTime.TryGetValue(id, out var lastAccess))
            {
                Console.WriteLine($"[VaultKeyStore] Checking expiration for vault {id}. Last access: {lastAccess}, Now: {DateTime.UtcNow}");
                Console.WriteLine($"[VaultKeyStore] Will expire in {_timeoutDuration - (DateTime.UtcNow - lastAccess)}");
                if (DateTime.UtcNow - lastAccess > _timeoutDuration)
                {
                    Lock(id);
                }
            }
        }

        private void Lock(Guid id)
        {
            _unlockedKeys.Remove(id);
            _decryptedNames.Remove(id); 
            _lastAccessTime.Remove(id);
        }
    }
}