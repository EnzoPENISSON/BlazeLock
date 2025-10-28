using Microsoft.EntityFrameworkCore;
using BlazeLock.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace BlazeLock.API.Controllers;

public static class LogController
{
    public static void MapLogEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Log").WithTags(nameof(Log));

        group.MapGet("/", async (BlazeLockContext db) =>
        {
            return await db.Logs.ToListAsync();
        })
        .WithName("GetAllLogs")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Log>, NotFound>> (Guid idlog, BlazeLockContext db) =>
        {
            return await db.Logs.AsNoTracking()
                .FirstOrDefaultAsync(model => model.IdLog == idlog)
                is Log model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetLogById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid idlog, Log log, BlazeLockContext db) =>
        {
            var affected = await db.Logs
                .Where(model => model.IdLog == idlog)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.IdLog, log.IdLog)
                    .SetProperty(m => m.Texte, log.Texte)
                    .SetProperty(m => m.Timestamp, log.Timestamp)
                    .SetProperty(m => m.IdCoffre, log.IdCoffre)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateLog")
        .WithOpenApi();

        group.MapPost("/", async (Log log, BlazeLockContext db) =>
        {
            db.Logs.Add(log);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Log/{log.IdLog}",log);
        })
        .WithName("CreateLog")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid idlog, BlazeLockContext db) =>
        {
            var affected = await db.Logs
                .Where(model => model.IdLog == idlog)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteLog")
        .WithOpenApi();
    }
}
