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
        private readonly IEncryptService _encryptService;
        private readonly IUtilisateurService _utilisateurService;

        public CoffreController(ICoffreService coffreService, IUtilisateurService utilisateurService, IEncryptService encryptService)
        {
            _coffreService = coffreService;
            _utilisateurService = utilisateurService;
            _encryptService = encryptService;
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

            dto.IdUtilisateur = userId;
            await _encryptService.HashMasterKey(dto);

            return CreatedAtAction(nameof(GetById), new { id = dto.IdCoffre }, dto);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(CoffreDto dto)
        {
            await _coffreService.Delete(dto);
            return Ok("Coffre supprimé");
        }

        [HttpPost("verify-password")]
        public async Task<IActionResult> VerifyMasterKeyPassword([FromBody] CoffreDto dto)
        {
            if (dto == null) return BadRequest("Invalid client request");

            var (userId, errorResult) = GetCurrentUserId();
            if (errorResult != null) return errorResult;

            var existingCoffre = await _coffreService.GetByIdAsync(dto.IdCoffre);

            if (existingCoffre == null)
            {
                return NotFound("Coffre not found");
            }

            if (existingCoffre.IdUtilisateur != userId) return Forbid();

            bool isValid = await _encryptService.VerifyMasterKey(
                dto.ClearPassword,
                existingCoffre.Salt,
                existingCoffre.HashMasterkey
            );

            if (isValid)
            {
                return Ok(isValid);
            }
            return Unauthorized(isValid);
        }

        private (Guid userId, IActionResult? error) GetCurrentUserId()
        {
            
            var userIdClaim = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");

            if (string.IsNullOrEmpty(userIdClaim))
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