using BlazeLock.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlazeLock.DbLib;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using BlazeLock.API.Extensions;

namespace BlazeLock.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/coffre")]
    public class CoffreController : ControllerBase
    {
        private readonly ICoffreService _coffreService;
        private readonly IUtilisateurService _utilisateurService;

        public CoffreController(ICoffreService coffreService, IUtilisateurService utilisateurService)
        {
            _coffreService = coffreService;
            _utilisateurService = utilisateurService;
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            try
            {
                var (userId, errorResult) = User.GetCurrentUserId();
                if (errorResult != null) return errorResult;

                var coffres = await _coffreService.GetByUtilisateurAsync(userId);

                if (coffres == null || !coffres.Any())
                    return NoContent();

                return Ok(coffres);
            }
            catch (Exception ex)
            {
                // Log l'exception ici si un logger est configuré
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération de vos coffres.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var coffres = await _coffreService.GetAllAsync();
                return Ok(coffres);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la récupération de tous les coffres.");
            }
        }

        [HttpGet("utilisateur/{id}")]
        public async Task<IActionResult> GetByUtilisateur(Guid id)
        {
            try
            {
                var coffres = await _coffreService.GetByUtilisateurAsync(id);
                if (coffres == null) return NotFound();
                return Ok(coffres);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération des coffres pour l'utilisateur {id}.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var coffre = await _coffreService.GetByIdAsync(id);

                if (coffre == null) return NotFound();

                var (userId, _) = User.GetCurrentUserId();
                if (coffre.IdUtilisateur != userId)
                    return Forbid(); 

                return Ok(coffre);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur est survenue lors de la récupération du coffre {id}.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CoffreDto dto)
        {
            try
            {
                var (userId, errorResult) = User.GetCurrentUserId();
                if (errorResult != null) return errorResult;

                var userExists = await _utilisateurService.ExistsAsync(userId);
                if (!userExists)
                {
                    return NotFound("Utilisateur non trouvé.");
                }

                dto.IdUtilisateur = userId;
                await _coffreService.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = dto.IdCoffre }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création du coffre.");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(CoffreDto dto)
        {
            try
            {
                // Idéalement, vérifier ici que l'utilisateur a le droit de supprimer ce coffre.
                await _coffreService.Delete(dto);
                return Ok("Coffre supprimé");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la suppression du coffre.");
            }
        }
    }
}