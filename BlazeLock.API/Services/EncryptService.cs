using BlazeLock.DbLib;
using Konscious.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
namespace BlazeLock.API.Services
{
    public class EncryptService : IEncryptService
    {
        private readonly ICoffreService _serviceCoffre;

        private const int SaltSize = 16;
        private const int KeySize = 32;

        private const int DegreeOfParallelism = 4;
        private const int Iterations = 4;
        private const int MemorySize = 65536;

        public EncryptService(ICoffreService service)
        {
            _serviceCoffre = service;
        }

        public async Task HashMasterKey(CoffreDto newCoffre)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] key = await ComputeArgon2id(newCoffre.ClearPassword!, salt);

            newCoffre.Salt = salt;
            newCoffre.IdCoffre = Guid.NewGuid();
            newCoffre.HashMasterkey = key;

            await _serviceCoffre.AddAsync(newCoffre);
        }

        public async Task<bool> VerifyMasterKey(string masterKey, byte[] storedSalt, byte[] storedHash)
        {
            byte[] keyToCheck = await ComputeArgon2id(masterKey, storedSalt);
            return CryptographicOperations.FixedTimeEquals(keyToCheck, storedHash);
        }

        public async Task<byte[]> GetDerivedKey(string masterKey, byte[] storedSalt)
        {
            return await ComputeArgon2id(masterKey, storedSalt);
        }
        private async Task<byte[]> ComputeArgon2id(string password, byte[] salt)
        {
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = DegreeOfParallelism;
                argon2.Iterations = Iterations;
                argon2.MemorySize = MemorySize;

                return await argon2.GetBytesAsync(KeySize);
            }
        }

        public bool IsValid(string password)
        {

            if (string.IsNullOrEmpty(password))
                return false;

            int score = 0;
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(char.IsUpper)) score++;
            if (password.Any(ch => !char.IsLetterOrDigit(ch))) score++;

            if (score < 2)
            {
                return false;
            }

            return true;
        }
    }
}

