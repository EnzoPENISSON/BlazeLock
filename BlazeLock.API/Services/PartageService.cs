using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.DbLib;
using Microsoft.EntityFrameworkCore;

namespace BlazeLock.API.Services
{
    public class PartageService : IPartageService
    {

        private readonly IPartageRepository _repository;

        public PartageService(IPartageRepository repository)
        {
            _repository = repository;
        }

        public async Task<HashSet<PartageDto>> GetAllAsync()
        {
            var partages = await _repository.GetAllAsync();

            var result = partages
                .Select(p => new PartageDto
                {
                    IdCoffre = p.IdCoffre,
                    IdUtilisateur = p.IdUtilisateur,
                    IsAdmin = p.IsAdmin
                })
                .ToHashSet();

            return result;
        }

        public async Task<HashSet<PartageDto>> GetByCoffreAsync(Guid id)
        {
            var partages = await _repository.GetByCoffreAsync(id);
            var result = partages
                .Select(p => new PartageDto
                {
                    IdCoffre = p.IdCoffre,
                    IdUtilisateur = p.IdUtilisateur,
                    Email = p.Utilisateur.email,
                    IsAdmin = p.IsAdmin
                })
                .ToHashSet();

            return result;
        }

        public async Task<HashSet<PartageDto>> GetByUtilisateurAsync(Guid id)
        {
            var partages = await _repository.GetByUtilisateurAsync(id);
            var result = partages
                .Select(p => new PartageDto
                {
                    IdCoffre = p.IdCoffre,
                    IdUtilisateur = p.IdUtilisateur,
                    IsAdmin = p.IsAdmin
                })
                .ToHashSet();

            return result;
        }

        public async Task AddAsync(PartageDto dto)
        {
            var entity = new Partage { 
                IdCoffre = dto.IdCoffre,
                IdUtilisateur = dto.IdUtilisateur,
                IsAdmin = dto.IsAdmin
            };
            await _repository.AddAsync(entity);
        }

        public async Task Delete(PartageDto dto)
        {
            var entity = new Partage
            {
                IdCoffre = dto.IdCoffre,
                IdUtilisateur = dto.IdUtilisateur,
                IsAdmin = dto.IsAdmin
            };
            await _repository.DeletePartage(entity);
        }
    }
}
