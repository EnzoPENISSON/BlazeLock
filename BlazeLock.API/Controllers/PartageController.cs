using BlazeLock.API.Services;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/partage")]
    public class PartageController : ControllerBase
    {
        private readonly IPartageService _service;

        public PartageController(IPartageService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var partages = await _service.GetAllAsync();
            return Ok(partages);
        }

        [HttpGet("utilisateur/{id}")]
        public async Task<IActionResult> GetByUtilisateur(Guid id)
        {
            var partages = await _service.GetByUtilisateurAsync(id);
            if (partages == null) return NotFound();
            return Ok(partages);
        }

        [HttpGet("coffre/{id}")]
        public async Task<IActionResult> GetByCoffre(Guid id)
        {
            var partages = await _service.GetByCoffreAsync(id);
            if (partages == null) return NotFound();
            return Ok(partages);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PartageDto dto)
        {
            await _service.AddAsync(dto);
            return Created();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(PartageDto dto)
        {
            await _service.Delete(dto);
            return Ok("Partage supprimé");
        }

    }
}