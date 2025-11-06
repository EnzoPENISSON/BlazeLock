using BlazeLock.API.Services;
using Microsoft.AspNetCore.Mvc;
using BlazeLock.DbLib;

namespace BlazeLock.API.Controllers;

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
        var entrees = await _service.GetAllAsync();
        return Ok(entrees);
    }

    [HttpGet("dossier/{id}")]
    public async Task<IActionResult> GetByDossier(Guid id)
    {
        var entrees = await _service.GetAllByDossierAsync(id);
        if (entrees == null) return NotFound();
        return Ok(entrees);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entrees = await _service.GetByIdAsync(id);
        if (entrees == null) return NotFound();
        return Ok(entrees);
    }

    [HttpGet("historique/{id}")]
    public async Task<IActionResult> GetByIdWithHistorique(Guid id)
    {
        var entrees = await _service.GetByIdWithHistoriaqueAsync(id);
        if (entrees == null) return NotFound();
        return Ok(entrees);
    }

    [HttpPost]
    public async Task<IActionResult> Create(EntreeDto dto)
    {
        await _service.AddAsync(dto);
        return Created();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(EntreeDto dto)
    {
        await _service.Delete(dto);
        return Ok("Entree supprimé");
    }

}
