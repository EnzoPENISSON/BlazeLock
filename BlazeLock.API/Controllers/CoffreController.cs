using Microsoft.EntityFrameworkCore;
using BlazeLock.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace BlazeLock.API.Controllers;

public static class CoffreController
{
    public static void MapCoffreEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Coffre").WithTags(nameof(Coffre));

        group.MapGet("/", async (BlazeLockContext db) =>
        {
            return await db.Coffres.ToListAsync();
        })
        .WithName("GetAllCoffres")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Coffre>, NotFound>> (Guid idcoffre, BlazeLockContext db) =>
        {
            return await db.Coffres.AsNoTracking()
                .FirstOrDefaultAsync(model => model.IdCoffre == idcoffre)
                is Coffre model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetCoffreById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid idcoffre, Coffre coffre, BlazeLockContext db) =>
        {
            var affected = await db.Coffres
                .Where(model => model.IdCoffre == idcoffre)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.IdCoffre, coffre.IdCoffre)
                    .SetProperty(m => m.Libelle, coffre.Libelle)
                    .SetProperty(m => m.HashMasterkey, coffre.HashMasterkey)
                    .SetProperty(m => m.Salt, coffre.Salt)
                    .SetProperty(m => m.IdUtilisateur, coffre.IdUtilisateur)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateCoffre")
        .WithOpenApi();

        group.MapPost("/", async (Coffre coffre, BlazeLockContext db) =>
        {
            db.Coffres.Add(coffre);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Coffre/{coffre.IdCoffre}",coffre);
        })
        .WithName("CreateCoffre")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid idcoffre, BlazeLockContext db) =>
        {
            var affected = await db.Coffres
                .Where(model => model.IdCoffre == idcoffre)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteCoffre")
        .WithOpenApi();
    }
}
