using BlazeLock.API.Extensions;
using BlazeLock.API.Helpers;
using BlazeLock.API.Models;
using BlazeLock.API.Services;
using BlazeLock.DbLib;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using System.Linq;

namespace BlazeLock.API.Controllers;

[Authorize]
[ApiController]
[Route("api/entree/{idCoffre}")]
[RequireVaultSession]
public class EntreeController : ControllerBase
{
    private readonly IEntreeService _entreeService;
    private readonly IDossierService _dossierService;
    private readonly ICoffreService _coffreService;
    private readonly ILogService _logService;

    public EntreeController(IEntreeService entreeService, IDossierService dossierService, ICoffreService coffreService, ILogService logService)
    {
        _entreeService = entreeService;
        _dossierService = dossierService;
        _coffreService = coffreService;
        _logService = logService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllByCoffre(Guid idCoffre)
    {
        try
        {
            var entrees = await _entreeService.GetAllByCoffreAsync(idCoffre);
            return Ok(entrees);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération des entrées.");
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid idCoffre, Guid id)
    {
        try
        {
            var entree = await _entreeService.GetByIdAsync(id);
            if (entree == null) return NotFound();
            
            await _entreeService.VerifyUserAccess(entree, User.GetCurrentUserId());
            //await _entreeService.AddLog(entree, User.GetCurrentUserId().userId, "Affichage de l'entrée " + entree.Libelle);

            return Ok(entree);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération de l'entrée {id}.");
        }
    }

    [HttpGet("dossier/{id}")]
    public async Task<IActionResult> GetByDossier(Guid idCoffre, Guid id)
    {
        try
        {
            var entrees = await _entreeService.GetAllByDossierAsync(idCoffre, id);
            if (entrees == null || !entrees.Any()) return NoContent();

            await _entreeService.VerifyUserAccess(entrees.First(), User.GetCurrentUserId());
            await _coffreService.AddLog(idCoffre, User.GetCurrentUserId().userId, "Affichage des entrées du dossier ");
            return Ok(entrees);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération des entrées pour le dossier {id}.");
        }
    }

    [HttpGet("historique/{id}")]
    public async Task<IActionResult> GetByIdWithHistorique(Guid idCoffre, Guid id)
    {
        try
        {
            var entrees = await _entreeService.GetByIdWithHistoriqueAsync(idCoffre, id);
            if (entrees == null) return NotFound();

            return Ok(entrees);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération de l'historique pour l'entrée {id}.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid idCoffre, EntreeDto dto)
    {
        try
        {
            await _entreeService.VerifyUserAccess(dto, User.GetCurrentUserId());

            await _entreeService.AddAsync(dto);
            // await _entreeService.AddLog(dto, User.GetCurrentUserId().userId, "création de l'entrée " + dto.Libelle);

            return CreatedAtAction(nameof(GetById),
                new { idCoffre = idCoffre, id = dto.IdEntree },
                dto);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création de l'entrée.");
        }
    }


    [HttpPost("dossier/{idEntree}/{idDossier}")]
    public async Task<IActionResult> MoveToFolder(Guid idCoffre, Guid idEntree, Guid idDossier)
    {
        try
        {
            await _entreeService.updateAsync(idEntree, idDossier);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid idCoffre, Guid id)
    {
        try
        {
            var entree = await _entreeService.GetByIdAsync(id);
            if (entree == null) return NotFound();
            await _entreeService.VerifyUserAccess(entree, User.GetCurrentUserId());
            await _entreeService.DeleteEntree(id);
            //await _entreeService.AddLog(entree, User.GetCurrentUserId().userId, "Suppression de l'entrée " + entree.Libelle);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la suppression de l'entrée.");
        }
    }
}