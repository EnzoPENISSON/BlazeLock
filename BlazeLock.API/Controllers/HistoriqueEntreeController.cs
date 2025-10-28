using Microsoft.EntityFrameworkCore;
using BlazeLock.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace BlazeLock.API.Controllers;

public static class HistoriqueEntreeController
{
    public static void MapHistoriqueEntreeEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/HistoriqueEntree").WithTags(nameof(HistoriqueEntree));

        group.MapGet("/", async (BlazeLockContext db) =>
        {
            return await db.HistoriqueEntrees.ToListAsync();
        })
        .WithName("GetAllHistoriqueEntrees")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<HistoriqueEntree>, NotFound>> (Guid idhistorique, BlazeLockContext db) =>
        {
            return await db.HistoriqueEntrees.AsNoTracking()
                .FirstOrDefaultAsync(model => model.IdHistorique == idhistorique)
                is HistoriqueEntree model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetHistoriqueEntreeById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid idhistorique, HistoriqueEntree historiqueEntree, BlazeLockContext db) =>
        {
            var affected = await db.HistoriqueEntrees
                .Where(model => model.IdHistorique == idhistorique)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.IdHistorique, historiqueEntree.IdHistorique)
                    .SetProperty(m => m.Libelle, historiqueEntree.Libelle)
                    .SetProperty(m => m.LibelleTag, historiqueEntree.LibelleTag)
                    .SetProperty(m => m.LibelleVi, historiqueEntree.LibelleVi)
                    .SetProperty(m => m.DateUpdate, historiqueEntree.DateUpdate)
                    .SetProperty(m => m.Username, historiqueEntree.Username)
                    .SetProperty(m => m.UsernameTag, historiqueEntree.UsernameTag)
                    .SetProperty(m => m.UsernameVi, historiqueEntree.UsernameVi)
                    .SetProperty(m => m.Url, historiqueEntree.Url)
                    .SetProperty(m => m.UrlTag, historiqueEntree.UrlTag)
                    .SetProperty(m => m.UrlVi, historiqueEntree.UrlVi)
                    .SetProperty(m => m.Password, historiqueEntree.Password)
                    .SetProperty(m => m.PasswordTag, historiqueEntree.PasswordTag)
                    .SetProperty(m => m.PasswordVi, historiqueEntree.PasswordVi)
                    .SetProperty(m => m.Commentaire, historiqueEntree.Commentaire)
                    .SetProperty(m => m.CommentaireTag, historiqueEntree.CommentaireTag)
                    .SetProperty(m => m.CommentaireVi, historiqueEntree.CommentaireVi)
                    .SetProperty(m => m.IdEntree, historiqueEntree.IdEntree)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateHistoriqueEntree")
        .WithOpenApi();

        group.MapPost("/", async (HistoriqueEntree historiqueEntree, BlazeLockContext db) =>
        {
            db.HistoriqueEntrees.Add(historiqueEntree);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/HistoriqueEntree/{historiqueEntree.IdHistorique}",historiqueEntree);
        })
        .WithName("CreateHistoriqueEntree")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid idhistorique, BlazeLockContext db) =>
        {
            var affected = await db.HistoriqueEntrees
                .Where(model => model.IdHistorique == idhistorique)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteHistoriqueEntree")
        .WithOpenApi();
    }
}
