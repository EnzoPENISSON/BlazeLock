using BlazeLock.API.Models;
using BlazeLock.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlazeLock.DbLib;

namespace BlazeLock.API.Controllers
{
    [ApiController]
    [Route("api/utilisateur")]
    public class UtilisateurController : ControllerBase
    {
        private readonly IUtilisateurService _serviceUtilisateur;

        public UtilisateurController(IUtilisateurService service)
        {
            _serviceUtilisateur = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var utilisateurs = await _serviceUtilisateur.GetAllAsync();
            return Ok(utilisateurs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var utilisateur = await _serviceUtilisateur.GetByIdAsync(id);
            if (utilisateur == null) return NotFound();
            return Ok(utilisateur);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UtilisateurDto dto)
        {
            if(_serviceUtilisateur.GetByIdAsync(dto.IdUtilisateur).Result != null)
            {
                return Ok("L'utilisateur existe déjà.");
            }
            await _serviceUtilisateur.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.IdUtilisateur }, dto);

        }
    }
}
