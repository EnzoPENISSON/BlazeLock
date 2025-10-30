using BlazeLock.API.Services;
using Microsoft.AspNetCore.Mvc;
using BlazeLock.DbLib;

namespace BlazeLock.API.Controllers;

[ApiController]
[Route("api/coffre")]
public class CoffreController : ControllerBase
{
    private readonly ICoffreService _service;

    public CoffreController(ICoffreService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var coffres = await _service.GetAllAsync();
        return Ok(coffres);
    }

    [HttpGet("utilisateur/{id}")]
    public async Task<IActionResult> GetByUtilisateur(Guid id)
    {
        var coffres = await _service.GetByUtilisateurAsync(id);
        if (coffres == null) return NotFound();
        return Ok(coffres);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var coffres = await _service.GetByIdAsync(id);
        if (coffres == null) return NotFound();
        return Ok(coffres);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CoffreDto dto)
    {
        await _service.AddAsync(dto);
        return Created();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(CoffreDto dto)
    {
        await _service.Delete(dto);
        return Ok("Coffre supprimé");
    }

}
