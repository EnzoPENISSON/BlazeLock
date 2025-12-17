using BlazeLock.API.Models;
using BlazeLock.API.Repositories;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazeLock.API.Services
{
    public class LogService : ILogService
    {

        private readonly ILogRepository _logRepository;
        private readonly IPartageRepository _partageRepository;
        private readonly ICoffreRepository _coffreRepository;

        public LogService(ILogRepository logRepository, IPartageRepository partageRepository, ICoffreRepository coffreRepository)
        {
            _logRepository = logRepository;
            _partageRepository = partageRepository;
            _coffreRepository = coffreRepository;
        }

        public async Task<HashSet<LogDto>> GetAllAsync()
        {
            var logs = await _logRepository.GetAllAsync();

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
            var logs = await _logRepository.GetByCoffreAsync(id);
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

        public async Task<PagedResultDto<LogDto>> GetByCoffrePagedAsync(Guid id, int pageNumber, int pageSize)
        {
            var (logs, totalCount) = await _logRepository.GetByCoffrePagedAsync(id, pageNumber, pageSize);

            var logDtos = logs.Select(l => new LogDto
            {
                IdCoffre = l.IdCoffre,
                Timestamp = l.Timestamp,
                IdUtilisateur = l.IdUtilisateur,
                Texte = l.Texte
            }).ToList();

            return new PagedResultDto<LogDto>
            {
                Items = logDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task Add(Guid idCoffre, Guid idUtilisateur, string message)
        {
            var entity = new Log {
                IdCoffre = idCoffre,
                IdUtilisateur = idUtilisateur,
                Texte = message,
                Timestamp = DateTime.UtcNow
            };
            await _logRepository.AddAsync(entity);
        }

        public async Task<IActionResult?> VerifyUserAccess(Guid idCoffre, (Guid, IActionResult?) utilisateur)
        {

            var (userId, errorResult) = utilisateur;
            if (errorResult != null) return errorResult;

            var coffre = await _coffreRepository.GetByIdAsync(idCoffre);
            if (coffre == null) return new BadRequestObjectResult("Le coffre associé à cette entrée n'a pas été trouvé.");

            if (coffre.IdUtilisateur == userId) return null;

            var isAdminPartage = await _partageRepository.IsCoffreAdmin(idCoffre, userId);
            if (isAdminPartage) return null;

            return new UnauthorizedObjectResult("Utilisateur non autorisé");
        }
    }
}
