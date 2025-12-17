using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Services
{
    public class CoffreService : ICoffreService
    {

        private readonly ICoffreRepository _coffreRepository;
        private readonly ILogRepository _logRepository;

        public CoffreService(ICoffreRepository coffreRepository, ILogRepository logRepository)
        {
            _coffreRepository = coffreRepository;
            _logRepository = logRepository;
        }

        public async Task<HashSet<CoffreDto>> GetAllAsync()
        {
            var partages = await _coffreRepository.GetAllAsync();

            var result = partages
                .Select(c => new CoffreDto
                {
                    IdCoffre = c.IdCoffre,
                    IdUtilisateur = c.IdUtilisateur,
                    Libelle = c.Libelle,
                    HashMasterkey = c.HashMasterkey,
                    Salt = c.Salt
                })
                .ToHashSet();

            return result;
        }

        public async Task<HashSet<CoffreDto>> GetByUtilisateurAsync(Guid idUtilisateur)
        {
            var partages = await _coffreRepository.GetByUtilisateurAsync(idUtilisateur);

            var result = partages
                .Select(c => new CoffreDto
                {
                    IdCoffre = c.IdCoffre,
                    IdUtilisateur = c.IdUtilisateur,
                    Libelle = c.Libelle,
                    HashMasterkey = c.HashMasterkey,
                    Salt = c.Salt
                })
                .ToHashSet();

            return result;
        }

        public async Task<CoffreDto?> GetByIdAsync(Guid id)
        {
            var result = await _coffreRepository.GetByIdAsync(id);

            if (result == null) return null;

            return new CoffreDto {
                IdCoffre = result.IdCoffre,
                IdUtilisateur = result.IdUtilisateur,
                Libelle = result.Libelle,
                HashMasterkey = result.HashMasterkey,
                Salt = result.Salt
            };
        }

        public async Task AddAsync(CoffreDto dto)
        {
            var entity = new Coffre
            {
                IdCoffre = Guid.NewGuid(),
                IdUtilisateur = dto.IdUtilisateur,
                Libelle = dto.Libelle,
                HashMasterkey = dto.HashMasterkey,
                Salt = dto.Salt
            };
            await _coffreRepository.AddAsync(entity);

        }

        public async Task Delete(CoffreDto dto)
        {
            var entity = new Coffre
            {
                IdCoffre = dto.IdCoffre,
                IdUtilisateur = dto.IdUtilisateur,
                Libelle = dto.Libelle,
                HashMasterkey = dto.HashMasterkey,
                Salt = dto.Salt
            };
            await _coffreRepository.DeleteCoffre(entity);
        }

        public async Task<IActionResult?> VerifyUserAccess(CoffreDto coffreDto, (Guid, IActionResult?) utilisateur)
        {

            var (userId, errorResult) = utilisateur;
            if (errorResult != null) return errorResult;
            
            if (coffreDto.IdUtilisateur != userId) return new UnauthorizedObjectResult("Utilisateur non autorisé");

            return null;
        }

        public async Task AddLog(Guid idCoffre, Guid idUtilisateur, string message)
        {
            var entity = new Log
            {
                IdCoffre = idCoffre,
                IdUtilisateur = idUtilisateur,
                Texte = message,
                Timestamp = DateTime.UtcNow
            };
            await _logRepository.AddAsync(entity);
        }

    }
}
