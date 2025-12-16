using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.DbLib;

namespace BlazeLock.API.Services
{
    public class DossierService : IDossierService
    {

        private readonly IDossierRepository _repository;

        public DossierService(IDossierRepository repository)
        {
            _repository = repository;
        }

        public async Task<HashSet<DossierDto>> GetAllAsync()
        {
            var partages = await _repository.GetAllAsync();

            var result = partages
                .Select(d => new DossierDto
                {
                    IdDossier = d.IdDossier,
                    Libelle = d.Libelle,
                    IdCoffre = d.IdCoffre,
                })
                .ToHashSet();

            return result;
        }

        public async Task<HashSet<DossierDto>> GetByCoffreAsync(Guid idCoffre)
        {
            var partages = await _repository.GetByCoffreAsync(idCoffre);

            var result = partages
                .Select(d => new DossierDto
                {
                    IdDossier = d.IdDossier,
                    Libelle = d.Libelle,
                    IdCoffre = d.IdCoffre,
                })
                .ToHashSet();

            return result;
        }

        public async Task<DossierDto?> GetByIdAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null) return null;

            return new DossierDto {
                IdDossier = result.IdDossier,
                Libelle = result.Libelle,
                IdCoffre = result.IdCoffre,
            };
        }

        public async Task AddAsync(DossierDto dto)
        {
            var entity = new Dossier
            {
                IdDossier = Guid.NewGuid(),
                Libelle = dto.Libelle,
                IdCoffre = dto.IdCoffre,
            };
            await _repository.AddAsync(entity);

        }

        public async Task Delete(DossierDto dto)
        {
            var entity = new Dossier
            {
                IdDossier = dto.IdDossier,
                Libelle = dto.Libelle,
                IdCoffre = dto.IdCoffre,
            };
            await _repository.DeleteDossier(entity);
        }

    }
}
