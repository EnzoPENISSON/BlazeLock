using Microsoft.EntityFrameworkCore;
using BlazeLock.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace BlazeLock.API.Controllers;

public static class PartageController
{
    public static void MapPartageEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Partage").WithTags(nameof(Partage));

        group.MapGet("/", async (BlazeLockContext db) =>
        {
            return await db.Partages.ToListAsync();
        })
        .WithName("GetAllPartages")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Partage>, NotFound>> (Guid idutilisateur, BlazeLockContext db) =>
        {
            return await db.Partages.AsNoTracking()
                .FirstOrDefaultAsync(model => model.IdUtilisateur == idutilisateur)
                is Partage model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetPartageById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid idutilisateur, Partage partage, BlazeLockContext db) =>
        {
            var affected = await db.Partages
                .Where(model => model.IdUtilisateur == idutilisateur)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.IdUtilisateur, partage.IdUtilisateur)
                    .SetProperty(m => m.IdCoffre, partage.IdCoffre)
                    .SetProperty(m => m.IsAdmin, partage.IsAdmin)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdatePartage")
        .WithOpenApi();

        group.MapPost("/", async (Partage partage, BlazeLockContext db) =>
        {
            db.Partages.Add(partage);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Partage/{partage.IdUtilisateur}",partage);
        })
        .WithName("CreatePartage")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid idutilisateur, BlazeLockContext db) =>
        {
            var affected = await db.Partages
                .Where(model => model.IdUtilisateur == idutilisateur)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeletePartage")
        .WithOpenApi();
    }
}
