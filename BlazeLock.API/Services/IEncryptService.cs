using BlazeLock.DbLib;

namespace BlazeLock.API.Services
{
    public interface IEncryptService
    {
        public Task HashMasterKey(CoffreDto newCoffre);
        public Task<bool> VerifyMasterKey(string masterKey, byte[] storedSalt, byte[] storedHash);
    }
}
