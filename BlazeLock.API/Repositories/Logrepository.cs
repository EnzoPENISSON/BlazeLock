using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace BlazeLock.API.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly BlazeLockContext _context;

        public LogRepository(BlazeLockContext context)
        {
            _context = context;
        }

        public async Task<HashSet<Log>> GetAllAsync()
        {
            var coffres = await _context.Logs.ToListAsync();
            return coffres.ToHashSet();
        }

        public async Task<Log?> GetByIdAsync(Guid idLog)
        {
            return await _context.Logs.FindAsync(idLog);
        }

        public async Task<HashSet<Log>> GetByCoffreAsync(Guid idCoffre)
        {
            var logs = await _context.Logs
                .Where(p => p.IdCoffre == idCoffre)
                .AsNoTracking()
                .ToListAsync();

            return logs.ToHashSet();
        }

        public async Task AddAsync(Log log)
        {
            await _context.Logs.AddAsync(log);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
