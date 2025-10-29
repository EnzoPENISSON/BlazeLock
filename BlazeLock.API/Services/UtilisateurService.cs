using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using DbLib;

namespace BlazeLock.API.Services
{
    public class UtilisateurService : IUtilisateurService
    {

        private readonly IUtilisateurRepository _repository;

        public UtilisateurService(IUtilisateurRepository repository)
        {
            _repository = repository;
        }

        public async Task<HashSet<UtilisateurDto>> GetAllAsync()
        {
            var utilisateurs = await _repository.GetAllAsync();

            var result = utilisateurs
                .Select(u => new UtilisateurDto
                {
                    IdUtilisateur = u.IdUtilisateur
                })
                .ToHashSet();

            return result;
        }

        public async Task<UtilisateurDto?> GetByIdAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return null;

            return new UtilisateurDto { IdUtilisateur = user.IdUtilisateur };
        }

        public async Task AddUtilisateurAsync(UtilisateurDto dto)
        {
            var entity = new Utilisateur { IdUtilisateur = dto.IdUtilisateur };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }
    }
}
