using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.DbLib;
using Microsoft.EntityFrameworkCore;

namespace BlazeLock.API.Services
{
    public class LogService : ILogService
    {

        private readonly ILogRepository _repository;

        public LogService(ILogRepository repository)
        {
            _repository = repository;
        }

        public async Task<HashSet<LogDto>> GetAllAsync()
        {
            var logs = await _repository.GetAllAsync();

            var result = logs
                .Select(l => new LogDto
                {
                    IdCoffre = l.IdCoffre,
                    Timestamp = l.Timestamp,
                    IdUtilisateur = l.IdUtilisateur,
                    Texte = l.Texte
                })
                .ToHashSet();

            return result;
        }

        public async Task<HashSet<LogDto>> GetByCoffreAsync(Guid id)
        {
            var logs = await _repository.GetByCoffreAsync(id);
            var result = logs
                .Select(l => new LogDto
                {
                    IdCoffre = l.IdCoffre,
                    Timestamp = l.Timestamp,
                    IdUtilisateur = l.IdUtilisateur,
                    Texte = l.Texte
                })
                .ToHashSet();

            return result;
        }

        public async Task Add(Guid idCoffre, Guid idUtilisateur, string message)
        {
            var entity = new Log {
                IdCoffre = idCoffre,
                IdUtilisateur = idUtilisateur,
                Texte = message,
                Timestamp = DateTime.UtcNow
            };
            await _repository.AddAsync(entity);
        }
    }
}
