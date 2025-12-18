using BlazeLock.API.Extensions;
using BlazeLock.API.Models;
using BlazeLock.API.Services;
using BlazeLock.DbLib;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BlazeLock.API.Controllers;

[Authorize]
[ApiController]
[Route("api/entree")]
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
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var entrees = await _entreeService.GetAllAsync();
            return Ok(entrees);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération des entrées.");
        }
    }

    [HttpGet("dossier/{idCoffre}/{idDossier}")]
    public async Task<IActionResult> GetByDossier(Guid idCoffre,Guid idDossier)
    {
        try
        {
            var entrees = await _entreeService.GetAllByDossierAsync(idCoffre, idDossier);
            if (entrees == null || !entrees.Any()) return NoContent();

            await _entreeService.VerifyUserAccess(entrees.First(), User.GetCurrentUserId());
            await _coffreService.AddLog(idCoffre, User.GetCurrentUserId().userId, "Affichage des entrées du dossier ");
            return Ok(entrees);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération des entrées pour le dossier {idDossier}.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var entree = await _entreeService.GetByIdAsync(id); // Id coffre Empty
            if (entree == null) return NotFound();
            Console.WriteLine("Entree trouvée : " + entree.IdEntree);   

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

    [HttpGet("historique/{id}")]
    public async Task<IActionResult> GetByIdWithHistorique(Guid id)
    {
        try
        {
            var entrees = await _entreeService.GetByIdWithHistoriaqueAsync(id);
            if (entrees == null) return NotFound();

            return Ok(entrees);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération de l'historique pour l'entrée {id}.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(EntreeDto dto)
    {
        try
        {
            await _entreeService.VerifyUserAccess(dto, User.GetCurrentUserId());

            await _entreeService.AddAsync(dto);
            //await _entreeService.AddLog(dto, User.GetCurrentUserId().userId, "création de l'entrée " + dto.Libelle);

            return CreatedAtAction(nameof(GetById), new { id = dto.IdEntree }, dto);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création de l'entrée.");
        }
    }


    [HttpPost("dossier/{idEntree}/{idDossier}")]
    public async Task<IActionResult> Update(Guid idEntree, Guid idDossier)
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

    [HttpGet("coffre/{idCoffre}")]
    public async Task<IActionResult> GetByCoffre(Guid idCoffre)
    {
        var entrees = await _entreeService.GetAllByCoffreAsync(idCoffre);
        if (entrees.Any())
        {
            await _entreeService.AddLog(entrees.First(), User.GetCurrentUserId().userId, "Affichage des entrées");
        }

        return Ok(entrees);
    }
}