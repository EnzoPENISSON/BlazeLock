using BlazeLock.API.Extensions;
using BlazeLock.API.Helpers;
using BlazeLock.API.Services;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazeLock.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/partage")]
    [RequireVaultSession]
    public class PartageController : ControllerBase
    {
        private readonly IPartageService _service;
        private readonly ICoffreService _coffreService;

        public PartageController(IPartageService service, ICoffreService coffreService)
        {
            _service = service;
            _coffreService = coffreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var partages = await _service.GetAllAsync();
                return Ok(partages);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération des partages.");
            }
        }

        [HttpGet("utilisateur/{id}")]
        public async Task<IActionResult> GetByUtilisateur(Guid id)
        {
            try
            {
                var partages = await _service.GetByUtilisateurAsync(id);
                if (partages == null || !partages.Any()) return NoContent();
                return Ok(partages);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération des partages pour l'utilisateur {id}.");
            }
        }

        [HttpGet("coffre/{id}")]
        public async Task<IActionResult> GetByCoffre(Guid id)
        {
            try
            {
                var partages = await _service.GetByCoffreAsync(id);
                if (partages == null || !partages.Any()) return NoContent();
                return Ok(partages);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération des partages pour le coffre {id}.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(PartageDto dto)
        {
            try
            {
                await _service.AddAsync(dto);
                return Created(string.Empty, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création du partage.");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(PartageDto dto)
        {
            try
            {
                var (userId, errorResult) = User.GetCurrentUserId();
                if (errorResult != null) return errorResult;

                var coffre = await _coffreService.GetByIdAsync(dto.IdCoffre);
                if (coffre == null)
                {
                    return NotFound("Le coffre associé à ce partage n'a pas été trouvé.");
                }

                if (coffre.IdUtilisateur != userId)
                {
                    return Forbid();
                }

                await _service.Delete(dto);
                return Ok("Partage supprimé");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la suppression du partage.");
            }
        }

    }
}