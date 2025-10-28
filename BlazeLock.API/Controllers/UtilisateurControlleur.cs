using BlazeLock.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;

namespace BlazeLock.API.Controllers
{
    public static class UtilisateurControlleur
    {
	public static void MapUtilisateurEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Utilisateur").WithTags(nameof(Utilisateur));

        group.MapGet("/", async (BlazeLockContext db) =>
        {
            return await db.Utilisateurs.ToListAsync();
        })
        .WithName("GetAllUtilisateurs")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Utilisateur>, NotFound>> (Guid idutilisateur, BlazeLockContext db) =>
        {
            return await db.Utilisateurs.AsNoTracking()
                .FirstOrDefaultAsync(model => model.IdUtilisateur == idutilisateur)
                is Utilisateur model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetUtilisateurById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid idutilisateur, Utilisateur utilisateur, BlazeLockContext db) =>
        {
            var affected = await db.Utilisateurs
                .Where(model => model.IdUtilisateur == idutilisateur)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.IdUtilisateur, utilisateur.IdUtilisateur)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateUtilisateur")
        .WithOpenApi();

        group.MapPost("/", async (Utilisateur utilisateur, BlazeLockContext db) =>
        {
            var exists = await db.Utilisateurs.AnyAsync(u => u.IdUtilisateur == utilisateur.IdUtilisateur);
            if (exists)
            {
                return Results.Ok();
            }
            db.Utilisateurs.Add(utilisateur);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Utilisateur/{utilisateur.IdUtilisateur}", utilisateur);
        })
        .WithName("CreateUtilisateur")
        .WithOpenApi();

            group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid idutilisateur, BlazeLockContext db) =>
        {
            var affected = await db.Utilisateurs
                .Where(model => model.IdUtilisateur == idutilisateur)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteUtilisateur")
        .WithOpenApi();
    }
        
        
    }
}
