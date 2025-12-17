using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Services
{
    public class DossierService : IDossierService
    {

        private readonly IDossierRepository _dossierRepository;
        private readonly ICoffreRepository _coffreRepository;
        private readonly ILogRepository _logRepository;

        public DossierService(IDossierRepository dossierRepository, ICoffreRepository coffreRepository, ILogRepository logRepository)
        {
            _dossierRepository = dossierRepository;
            _coffreRepository = coffreRepository;
            _logRepository = logRepository;
        }

        public async Task<HashSet<DossierDto>> GetAllAsync()
        {
            var partages = await _dossierRepository.GetAllAsync();

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
            var partages = await _dossierRepository.GetByCoffreAsync(idCoffre);

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
            var result = await _dossierRepository.GetByIdAsync(id);

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
            await _dossierRepository.AddAsync(entity);

        }

        public async Task Delete(DossierDto dto)
        {
            var entity = new Dossier
            {
                IdDossier = dto.IdDossier,
                Libelle = dto.Libelle,
                IdCoffre = dto.IdCoffre,
            };
            await _dossierRepository.DeleteDossier(entity);
        }

        public async Task<IActionResult?> VerifyUserAccess(DossierDto dossierDto, (Guid, IActionResult?) utilisateur)
        {

            var (userId, errorResult) = utilisateur;
            if (errorResult != null) return errorResult;

            var coffre = await _coffreRepository.GetByIdAsync(dossierDto.IdCoffre);
            if (coffre == null) return new BadRequestObjectResult("Le coffre associé à ce dossier n'a pas été trouvé.");

            if (coffre.IdUtilisateur != userId) return new UnauthorizedObjectResult("Utilisateur non autorisé");

            return null;
        }

        public async Task AddLog(DossierDto dossier, Guid idUtilisateur, string message)
        {
            var entity = new Log
            {
                IdCoffre = dossier.IdCoffre,
                IdUtilisateur = idUtilisateur,
                Texte = message,
                Timestamp = DateTime.UtcNow
            };
            await _logRepository.AddAsync(entity);
        }
    }
}
