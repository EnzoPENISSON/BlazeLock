using BlazeLock.DbLib;
using System.Security.Cryptography;

namespace BlazeLock.API.Services
{
    public class EncryptService
    {
        private readonly ICoffreService _serviceCoffre;

        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 310_000;


        public EncryptService(ICoffreService service)
        {
            _serviceCoffre = service;
        }

        public void HashMasterKey(string masterKey, string libelle)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                masterKey,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize
            );

            CoffreDto newCoffre = new CoffreDto
            {
                IdCoffre = Guid.NewGuid(),
                Salt = salt,
                HashMasterkey = key,
                Libelle = libelle,

            };

            try
            {
                _serviceCoffre.AddAsync(newCoffre);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'ajout du coffre : " + ex.Message);
            }
        }

        public bool VerifyMasterKey(string masterKey, byte[] storedSalt, byte[] storedHash)
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

