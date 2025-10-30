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
                .Select(p => new LogDto
                {
                    IdCoffre = p.IdCoffre,
                    Texte = p.Texte
                })
                .ToHashSet();

            return result;
        }

        public async Task<HashSet<LogDto>> GetByCoffreAsync(Guid id)
        {
            var logs = await _repository.GetByCoffreAsync(id);
            var result = logs
                .Select(p => new LogDto
                {
                    IdCoffre = p.IdCoffre,
                    Timestamp = p.Timestamp,
                    Texte = p.Texte
                })
                .ToHashSet();

            return result;
        }

        public async Task AddAsync(LogDto dto)
        {
            var entity = new Log { 
                IdCoffre = dto.IdCoffre,
                Texte = dto.Texte
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }
    }
}
