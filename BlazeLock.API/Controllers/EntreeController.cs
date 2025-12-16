using BlazeLock.API.Extensions;
using BlazeLock.API.Services;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Controllers;

[Authorize]
[ApiController]
[Route("api/entree")]
public class EntreeController : ControllerBase
{
    private readonly IEntreeService _entreeService;
    private readonly IDossierService _dossierService;
    private readonly ICoffreService _coffreService;

    public EntreeController(IEntreeService entreeService, IDossierService dossierService, ICoffreService coffreService)
    {
        _entreeService = entreeService;
        _dossierService = dossierService;
        _coffreService = coffreService;
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
            // Idéalement, loguer l'exception ex
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération des entrées.");
        }
    }

    [HttpGet("dossier/{id}")]
    public async Task<IActionResult> GetByDossier(Guid id)
    {
        try
        {
            DossierDto? dossier = await _dossierService.GetByIdAsync(id);

            var entrees = await _entreeService.GetAllByDossierAsync(id);
            if (entrees == null || !entrees.Any()) return NoContent();

            await _dossierService.VerifyUserAccess(dossier, User.GetCurrentUserId());
            return Ok(entrees);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération des entrées pour le dossier {id}.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var entree = await _entreeService.GetByIdAsync(id);
            if (entree == null) return NotFound();

            await _entreeService.VerifyUserAccess(entree, User.GetCurrentUserId());
            return Ok(entree);
        }
        catch (Exception ex)
        {
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
            // Retourne un 201 Created avec l'URL pour récupérer la nouvelle ressource
            return CreatedAtAction(nameof(GetById), new { id = dto.IdEntree }, dto);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création de l'entrée.");
        }
    }

    //[HttpDelete]
    //public async Task<IActionResult> Delete(EntreeDto dto)
    //{
    //    await _service.Delete(dto);
    //    return Ok("Entree supprimé");
    //}

}