using BlazeLock.API.Models;
using BlazeLock.DbLib;
using System.Security.Cryptography;

namespace BlazeLock.API.Services
{
    public class EncryptService : IEncryptService
    {
        private readonly ICoffreService _serviceCoffre;

        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 310_000;


        public EncryptService(ICoffreService service)
        {
            _serviceCoffre = service;
        }

        public async Task HashMasterKey(CoffreDto newCoffre)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                newCoffre.ClearPassword!,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize
            );

            newCoffre.Salt = salt;
            newCoffre.IdCoffre = Guid.NewGuid();
            newCoffre.HashMasterkey = key;

            try
            {
                _serviceCoffre.AddAsync(newCoffre);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'ajout du coffre : " + ex.Message);
            }
        }

        public async Task<bool> VerifyMasterKey(string masterKey, byte[] storedSalt, byte[] storedHash)
        {
            byte[] keyToCheck = Rfc2898DeriveBytes.Pbkdf2(
                masterKey,
                storedSalt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize
            );
            return CryptographicOperations.FixedTimeEquals(keyToCheck, storedHash);
        }

    }
}

