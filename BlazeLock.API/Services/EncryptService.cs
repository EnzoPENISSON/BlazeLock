using BlazeLock.DbLib;
using Konscious.Security.Cryptography;
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

            byte[] key;

            byte[] passwordBytes = Encoding.UTF8.GetBytes(newCoffre.ClearPassword!);

            using (var argon2 = new Argon2id(passwordBytes))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = DegreeOfParallelism;
                argon2.Iterations = Iterations;
                argon2.MemorySize = MemorySize;

                key = await argon2.GetBytesAsync(KeySize);
            }

            newCoffre.Salt = salt;
            newCoffre.IdCoffre = Guid.NewGuid();
            newCoffre.HashMasterkey = key;

            try
            {
                await _serviceCoffre.AddAsync(newCoffre);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'ajout du coffre : " + ex.Message);
            }
        }

        public async Task<bool> VerifyMasterKey(string masterKey, byte[] storedSalt, byte[] storedHash)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(masterKey);

            byte[] keyToCheck;

            using (var argon2 = new Argon2id(passwordBytes))
            {
                argon2.Salt = storedSalt;
                argon2.DegreeOfParallelism = DegreeOfParallelism;
                argon2.Iterations = Iterations;
                argon2.MemorySize = MemorySize;

                keyToCheck = await argon2.GetBytesAsync(KeySize);
            }

            return CryptographicOperations.FixedTimeEquals(keyToCheck, storedHash);
        }

    }
}

