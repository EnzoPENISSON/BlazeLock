using BlazeLock.API.Models;
using BlazeLock.API.Services;
using DbLib;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Controllers
{
    [ApiController]
    [Route("api/utilisateur")]
    public class UtilisateurController : ControllerBase
    {
        private readonly IUtilisateurService _service;

        public UtilisateurController(IUtilisateurService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var utilisateurs = await _service.GetAllAsync();
            return Ok(utilisateurs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var utilisateur = await _service.GetByIdAsync(id);
            if (utilisateur == null) return NotFound();
            return Ok(utilisateur);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UtilisateurDto dto)
        {
            await _service.AddUtilisateurAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.IdUtilisateur }, dto);
        }
    }
}
