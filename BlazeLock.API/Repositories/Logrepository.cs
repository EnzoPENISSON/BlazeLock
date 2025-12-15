using BlazeLock.API.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace BlazeLock.API.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly IDbContextFactory<BlazeLockContext> _contextFactory;

        public LogRepository(IDbContextFactory<BlazeLockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<HashSet<Log>> GetAllAsync()
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var coffres = await context.Logs.AsNoTracking().ToListAsync();
            return coffres.ToHashSet();
        }

        public async Task<Log?> GetByIdAsync(Guid idLog)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            return await context.Logs.FindAsync(idLog);
        }

        public async Task<HashSet<Log>> GetByCoffreAsync(Guid idCoffre)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var logs = await context.Logs
                .Where(p => p.IdCoffre == idCoffre)
                .AsNoTracking()
                .ToListAsync();

            return logs.ToHashSet();
        }

        public async Task AddAsync(Log log)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            await context.Logs.AddAsync(log);
            await context.SaveChangesAsync();
        }
    }
}
