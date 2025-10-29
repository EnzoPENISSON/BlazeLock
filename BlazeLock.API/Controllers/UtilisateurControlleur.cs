using BlazeLock.API.Models;
using BlazeLock.API.Services;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Controllers
{
    [Authorize]
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
        [Authorize(Roles = Roles.User_Administrator)]
        public async Task<IActionResult> GetAll()
        {
            var utilisateurs = await _service.GetAllAsync();
            return Ok(utilisateurs);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Roles.User_Administrator)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var utilisateur = await _service.GetByIdAsync(id);
            if (utilisateur == null) return NotFound();
            return Ok(utilisateur);
        }

        [HttpPost]
        [Authorize(Roles = Roles.User_Administrator)]
        public async Task<IActionResult> Create(UtilisateurDto dto)
        {
            var existingUser = await this.GetById(dto.IdUtilisateur);
            if (existingUser == null)
            {
                await _service.AddUtilisateurAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = dto.IdUtilisateur }, dto);
            }
            return Ok();
        }
    }
}
