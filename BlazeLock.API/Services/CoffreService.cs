using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.DbLib;

namespace BlazeLock.API.Services
{
    public class CoffreService : ICoffreService
    {

        private readonly ICoffreRepository _repository;
        private readonly IDossierRepository _dossierRepository;

        public CoffreService(ICoffreRepository repository, IDossierRepository dossierRepository)
        {
            _repository = repository;
            _dossierRepository = dossierRepository;
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
            Guid idCoffre = Guid.NewGuid();

            var entity = new Coffre
            {
                IdCoffre = idCoffre,
                IdUtilisateur = dto.IdUtilisateur,
                Libelle = dto.Libelle,
                HashMasterkey = dto.HashMasterkey,
                Salt = dto.Salt
            };

            await _repository.AddAsync(entity);

            var newDefaultFolder = new Dossier
            {
                IdDossier = Guid.NewGuid(),
                Libelle = "Default",
                IdCoffre = idCoffre
            };

            await _dossierRepository.AddAsync(newDefaultFolder);
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

    }
}
