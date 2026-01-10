using BlazeLock.API.Extensions;
using BlazeLock.API.Helpers;
using BlazeLock.API.Models;
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
        private readonly ICoffreService _coffreService;
        private readonly ILogService _logService;

        public PartageController(IPartageService service, ICoffreService coffreService, ILogService logService)
        {
            _service = service;
            _coffreService = coffreService;
            _logService = logService;
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

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyPartages()
        {
            try
            {
                var (userId, errorResult) = User.GetCurrentUserId();
                if (errorResult != null) return errorResult;

                var partages = await _service.GetByUtilisateurAsync(userId);
                if (partages == null || !partages.Any()) return Ok(new List<PartageDto>());
                return Ok(partages);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération de vos partages.");
            }
        }

        [HttpGet("coffre/{idCoffre}")]
        [RequireVaultSession] // Ajouté ici car cette route nécessite un coffre déverrouillé
        public async Task<IActionResult> GetByCoffre(Guid idCoffre)
        {
            try
            {
                var partages = await _service.GetByCoffreAsync(idCoffre);
                if (partages == null || !partages.Any()) return Ok(new List<PartageDto>());
                return Ok(partages.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération des partages pour le coffre {idCoffre}.");
            }
        }

        [HttpPost("{idCoffre}")]
        [RequireVaultSession] // Ajouté ici
        public async Task<IActionResult> Create(PartageDto dto)
        {
            try
            {
                await _service.AddAsync(dto);
                await _logService.Add(dto.IdCoffre, User.GetCurrentUserId().userId, "Affichage des dossier du coffre");
                return Created(string.Empty, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création du partage.");
            }
        }

        [HttpDelete("{idCoffre}")]
        [RequireVaultSession] // Ajouté ici
        public async Task<IActionResult> Delete(Guid idCoffre, PartageDto dto)
        {
            try
            {
                if (idCoffre != dto.IdCoffre)
                {
                    return BadRequest("L'ID du coffre dans l'URL ne correspond pas à celui dans le corps de la requête.");
                }

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
                await _logService.Add(dto.IdCoffre, User.GetCurrentUserId().userId, $"Partage à l'utilisateur {dto.IdUtilisateur} supprimé ");
                return Ok("Partage supprimé");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la suppression du partage.");
            }
        }

    }
}