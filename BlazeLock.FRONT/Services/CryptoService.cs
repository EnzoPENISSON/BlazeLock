using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography; // Nécessaire pour PBKDF2 (Rfc2898DeriveBytes)
using Org.BouncyCastle.Crypto; // <--- C'est ici que se trouve InvalidCipherTextException
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace BlazeLock.FRONT.Services
{
    public class CryptoService
    {
        // Constantes techniques
        private const int KeySize = 32;       // 256 bits
        private const int NonceSize = 12;     // 12 octets (IV)
        private const int TagSize = 16;       // 16 octets (Tag)
        private const int MacSizeBits = 128;  // TagSize * 8 (16*8)
        private const int Iterations = 100000;

        /// <summary>
        /// Dérive la clé (PBKDF2) - Le standard .NET fonctionne bien ici.
        /// </summary>
        public byte[] DeriveKey(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(KeySize);
        }

        /// <summary>
        /// Chiffre avec AES-GCM via BouncyCastle (Compatible WebAssembly)
        /// </summary>
        public byte[] EncryptData(string plainText, byte[] key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            // 1. Générer le Nonce (IV) aléatoire
            var nonce = new byte[NonceSize];
            var random = new SecureRandom();
            random.NextBytes(nonce);

            // 2. Configurer le moteur AES-GCM
            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(key), MacSizeBits, nonce);
            cipher.Init(true, parameters); // true = Encrypt

            // 3. Exécuter le chiffrement
            // BouncyCastle produit : [CipherText] + [Tag]
            var outputSize = cipher.GetOutputSize(plainBytes.Length);
            var cipherTextWithTag = new byte[outputSize];
            int len = cipher.ProcessBytes(plainBytes, 0, plainBytes.Length, cipherTextWithTag, 0);
            cipher.DoFinal(cipherTextWithTag, len);

            // 4. Reformater pour notre standard : [IV (12)] + [Tag (16)] + [Cipher (reste)]
            // BouncyCastle met le Tag à la fin. On doit le déplacer pour respecter votre structure.
            var actualTag = new byte[TagSize];
            var actualCipher = new byte[cipherTextWithTag.Length - TagSize];

            // Extraire le Tag (qui est à la fin)
            Array.Copy(cipherTextWithTag, cipherTextWithTag.Length - TagSize, actualTag, 0, TagSize);
            // Extraire le Cipher (qui est au début)
            Array.Copy(cipherTextWithTag, 0, actualCipher, 0, actualCipher.Length);

            // Combiner : IV + Tag + Cipher
            var finalCombined = new byte[NonceSize + TagSize + actualCipher.Length];
            Buffer.BlockCopy(nonce, 0, finalCombined, 0, NonceSize);
            Buffer.BlockCopy(actualTag, 0, finalCombined, NonceSize, TagSize);
            Buffer.BlockCopy(actualCipher, 0, finalCombined, NonceSize + TagSize, actualCipher.Length);

            return finalCombined;
        }

        /// <summary>
        /// Déchiffre avec AES-GCM via BouncyCastle (Compatible WebAssembly)
        /// </summary>
        public string DecryptData(byte[] encryptedDataCombined, byte[] key)
        {
            if (encryptedDataCombined == null || encryptedDataCombined.Length < NonceSize + TagSize)
                throw new ArgumentException("Données invalides.");

            // 1. Découper notre format : [IV (12)] [Tag (16)] [CipherText]
            var nonce = new byte[NonceSize];
            var tag = new byte[TagSize];
            var cipherTextLen = encryptedDataCombined.Length - NonceSize - TagSize;
            var cipherText = new byte[cipherTextLen];

            Array.Copy(encryptedDataCombined, 0, nonce, 0, NonceSize);
            Array.Copy(encryptedDataCombined, NonceSize, tag, 0, TagSize);
            Array.Copy(encryptedDataCombined, NonceSize + TagSize, cipherText, 0, cipherTextLen);

            // 2. Reconstruire le format attendu par BouncyCastle : [CipherText] + [Tag]
            var bcInput = new byte[cipherText.Length + tag.Length];
            Buffer.BlockCopy(cipherText, 0, bcInput, 0, cipherText.Length);
            Buffer.BlockCopy(tag, 0, bcInput, cipherText.Length, tag.Length);

            // 3. Configurer le moteur
            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(key), MacSizeBits, nonce);
            cipher.Init(false, parameters); // false = Decrypt

            // 4. Exécuter le déchiffrement
            var plainBytes = new byte[cipher.GetOutputSize(bcInput.Length)];
            try
            {
                int len = cipher.ProcessBytes(bcInput, 0, bcInput.Length, plainBytes, 0);
                cipher.DoFinal(plainBytes, len);
            }
            catch (InvalidCipherTextException)
            {
                throw new UnauthorizedAccessException("Déchiffrement impossible : mot de passe incorrect ou données corrompues.");
            }

            // 5. Retourner le texte (nettoyage des null bytes éventuels)
            return Encoding.UTF8.GetString(plainBytes).TrimEnd('\0');
        }

        public bool VerifyMasterPassword(string inputPassword, byte[] storedHash, byte[] storedSalt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(inputPassword, storedSalt, Iterations, HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(KeySize);
            return computedHash.SequenceEqual(storedHash);
        }
    }
}