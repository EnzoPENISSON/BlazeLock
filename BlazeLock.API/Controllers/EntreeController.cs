using Microsoft.EntityFrameworkCore;
using BlazeLock.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace BlazeLock.API.Controllers;

public static class EntreeController
{
    public static void MapEntreeEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Entree").WithTags(nameof(Entree));

        group.MapGet("/", async (BlazeLockContext db) =>
        {
            return await db.Entrees.ToListAsync();
        })
        .WithName("GetAllEntrees")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Entree>, NotFound>> (Guid identree, BlazeLockContext db) =>
        {
            return await db.Entrees.AsNoTracking()
                .FirstOrDefaultAsync(model => model.IdEntree == identree)
                is Entree model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetEntreeById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid identree, Entree entree, BlazeLockContext db) =>
        {
            var affected = await db.Entrees
                .Where(model => model.IdEntree == identree)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.IdEntree, entree.IdEntree)
                    .SetProperty(m => m.DateCreation, entree.DateCreation)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateEntree")
        .WithOpenApi();

        group.MapPost("/", async (Entree entree, BlazeLockContext db) =>
        {
            db.Entrees.Add(entree);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Entree/{entree.IdEntree}",entree);
        })
        .WithName("CreateEntree")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid identree, BlazeLockContext db) =>
        {
            var affected = await db.Entrees
                .Where(model => model.IdEntree == identree)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteEntree")
        .WithOpenApi();
    }
}
