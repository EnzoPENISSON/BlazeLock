using BlazeLock.API.Models;
using BlazeLock.API.Services;
using BlazeLock.DbLib;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Controllers;

[Authorize]
[ApiController]
[Route("api/entree")]
public class EntreeController : ControllerBase
{
    private readonly IEntreeService _service;

    public EntreeController(IEntreeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var entrees = await _service.GetAllAsync();
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
            var entrees = await _service.GetAllByDossierAsync(idCoffre, idDossier);
            if (entrees == null || !entrees.Any()) return NoContent();
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
            var entree = await _service.GetByIdAsync(id);
            if (entree == null) return NotFound();
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
            var entrees = await _service.GetByIdWithHistoriaqueAsync(id);
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
            await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.IdEntree }, dto);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création de l'entrée.");
        }
    }


    [HttpPost("dossier/{idEntree}/{idDossier}")]
    public async Task<IActionResult> Update(Guid idEntree, Guid idDossier)
    {
        try
        {
            await _service.updateAsync(idEntree, idDossier);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue.");
        }
    }

    [HttpGet("coffre/{id}")]
    public async Task<IActionResult> GetByCoffre(Guid id)
    {
        var entrees = await _service.GetAllByCoffreAsync(id);
        return Ok(entrees);
    }

    //[HttpDelete]
    //public async Task<IActionResult> Delete(EntreeDto dto)
    //{
    //    await _service.Delete(dto);
    //    return Ok("Entree supprimé");
    //}

}