using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Services
{
    public class CoffreService : ICoffreService
    {

        private readonly ICoffreRepository _repository;

        public CoffreService(ICoffreRepository repository)
        {
            _repository = repository;
        }

        public async Task<HashSet<CoffreDto>> GetAllAsync()
        {
            var partages = await _repository.GetAllAsync();

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
            var partages = await _repository.GetByUtilisateurAsync(idUtilisateur);

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
            var result = await _repository.GetByIdAsync(id);

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
            await _repository.AddAsync(entity);

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
            await _repository.DeleteCoffre(entity);
        }

        public async Task<IActionResult?> VerifyUserAccess(CoffreDto coffreDto, (Guid, IActionResult?) utilisateur)
        {

            var (userId, errorResult) = utilisateur;
            if (errorResult != null) return errorResult;
            
            if (coffreDto.IdUtilisateur != userId) return new UnauthorizedObjectResult("Utilisateur non autorisé");

            return null;
        }

    }
}
